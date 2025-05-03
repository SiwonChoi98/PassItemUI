using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Progress = UnityEditor.Progress;

public class LumberPassUI : MonoBehaviour
{
    [Header("Currency")] 
    [SerializeField] private Text _gameMoneyText;
    [SerializeField] private Text _gemText;
    [SerializeField] private Text _upgradeText;
    [SerializeField] private Text _LevelUpPointText;

    private Action<CurrencyDataType> _onChangedCurrency;
    
    [Header("PassLevel")] 
    [SerializeField] private Text _passLevelText;
    [SerializeField] private Text _passExpText;
    [SerializeField] private Slider _passExpSlider;

    [Header("PassTime")]
    [SerializeField] private Text _passRemainingTimeText;
    
    [Header("PassItem")]
    [SerializeField] private ItemLumberPass _baseItemLumberPassPrefab;
    [SerializeField] private ScrollRect _scrollRect;
    private List<ItemLumberPass> _itemList;
    private float _offset;
    private float _itemHeight;
    
    [Header("PremiumPass")] 
    [SerializeField] private Button _premiumPassBtn;
    private async void Start()
    {
        await UniTask.WaitUntil(() => DataManager.Instance != null && DataManager.Instance.UserData != null);
        SpawnItem();
        SetContentHeight();
        SetLumberPassData();
        
        _scrollRect.onValueChanged.AddListener(_ => OnScrollChanged());
        _premiumPassBtn.onClick.AddListener(Btn_BuyPremiumPass);

        _onChangedCurrency += UpdateCurrency;
        
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            DataManager.Instance.AddPass(PassDataType.EXP, 10);
        }
    }

    private void OnEnable()
    {
        AddDelegate();
    }

    private void OnDisable()
    {
        RemoveDelegate();
    }

    private void AddDelegate()
    {
        if (DataManager.Instance != null)
        {
            DataManager.Instance.OnChangedExp += SetPassLevelData;
            DataManager.Instance.OnChangedLevel += SetItemListImage;
            
            DataManager.Instance.OnChangedPass += InitPassAction;
        }

        if (PassManager.Instance != null)
        {
            PassManager.Instance.OnChangedPassTime += UpdatePassRemainingTime;
        }
    }

    private void RemoveDelegate()
    {
        if (DataManager.Instance != null)
        {
            DataManager.Instance.OnChangedExp -= SetPassLevelData;
            DataManager.Instance.OnChangedLevel -= SetItemListImage;
            
            DataManager.Instance.OnChangedPass -= InitPassAction;
        }
        
        if (PassManager.Instance != null)
        {
            PassManager.Instance.OnChangedPassTime -= UpdatePassRemainingTime;
        }
    }
    
    private void UpdatePassRemainingTime(DateTime resetTime)
    {
        TimeSpan timeLeft = resetTime - DateTime.UtcNow;

        if (timeLeft.TotalSeconds <= 0)
        {
            _passRemainingTimeText.text = "RemainingTime : 0h 0m 0s";
            return;
        }
        else
        {
            int hours = (int)timeLeft.TotalHours;
            int minutes = timeLeft.Minutes;
            int seconds = timeLeft.Seconds;

            _passRemainingTimeText.text = $"RemainingTime : {hours:D2}h {minutes:D2}m {seconds:D2}s";
        }
    }
    
    private void Btn_BuyPremiumPass()
    {
        UserData userData = DataManager.Instance.UserData;
        if (userData.PassData.IsSpecialPassEnabled)
            return;

        DataManager.Instance.BuySpecialPass();
        _premiumPassBtn.gameObject.SetActive(false);

        SetItemListImage();
        
        Debug.Log("Buy Premium Pass");
    }

    private void InitPassAction()
    {
        SetPremiumPassBtnEnable();
        SetPassLevelData();
        SetItemListImage();
    }
    
    private void SetLumberPassData()
    {
        UserData userData = DataManager.Instance.UserData;
        
        _gameMoneyText.text = userData.CurrencyData.GameMoney.ToString();
        _gemText.text = userData.CurrencyData.Gem.ToString();
        _upgradeText.text = userData.CurrencyData.Upgrade.ToString();
        _LevelUpPointText.text = userData.CurrencyData.LevelUpPoint.ToString();
        
        SetPremiumPassBtnEnable();
        SetPassLevelData();
    }
    private void SetItemListImage()
    {
        for (int i = 0; i < _itemList.Count; i++)
        {
            _itemList[i].SetLockImage();
            _itemList[i].SetLevelLine();
            _itemList[i].SetLineLevelImage();
            _itemList[i].SetCheckImage();
        }
    }

    private void SetPremiumPassBtnEnable()
    {
        UserData userData = DataManager.Instance.UserData;
        
        if (userData.PassData.IsSpecialPassEnabled)
            _premiumPassBtn.gameObject.SetActive(false);
        else 
            _premiumPassBtn.gameObject.SetActive(true);
    }
    private void SetPassLevelData()
    {
        UserData userData = DataManager.Instance.UserData;
        
        _passLevelText.text = "Rank : " + userData.PassData.PassLevel;
        
        float userPassExp = userData.PassData.PassExp;
        float userNextLevelNeedExp = SpecDataManager.Instance.GetPassInfoData(userData.PassData.PassLevel).need_exp;
        
        _passExpText.text = $"<color=#FFFFEB>{userPassExp}</color><color=#FFFFEB>/{userNextLevelNeedExp}</color>";
        _passExpSlider.value = userPassExp / userNextLevelNeedExp;

        SetTargetItemLevelLine(userData);
    }

    private void SetTargetItemLevelLine(UserData userData)
    {
        if (userData?.PassData == null)
            return;

        int userLevel = userData.PassData.PassLevel;

        for (int i = 0; i < _itemList.Count; i++)
        {
            if (_itemList[i] == null || _itemList[i].Data == null)
                continue;

            int itemLevel = _itemList[i].Data.pass_level;

            if (userLevel == itemLevel || userLevel + 1 == itemLevel)
            {
                _itemList[i].SetLevelLine();
            }
        }
    }
    private void SetItemData(ItemLumberPass item, int key)
    {
        PassInfoData passInfoData = SpecDataManager.Instance.GetPassInfoData(key);
        
        item.SetData(passInfoData);
        
        item.OnRewardReceived = () => UpdateCurrency((CurrencyDataType)passInfoData.reward_idx);
        item.OnSpecialRewardReceived = () => UpdateCurrency((CurrencyDataType)passInfoData.special_reward_idx);
        
    }
    
    private void SpawnItem()
    {
        Debug.Log("SpawnItem");
        _itemHeight = _baseItemLumberPassPrefab.GetComponent<RectTransform>().rect.height;
        
        RectTransform scrollRect = _scrollRect.GetComponent<RectTransform>();
        _itemList = new List<ItemLumberPass>();

        int itemCount = (int)(scrollRect.rect.height / _itemHeight) + 1 + 2;

        for (int i = 0; i < itemCount; i++)
        {
            ItemLumberPass item = Instantiate(_baseItemLumberPassPrefab, _scrollRect.content);
            item.transform.localPosition = new Vector3(0, -i * _itemHeight);
            
            _itemList.Add(item);
            SetItemData(item, i+1);
        }

        _offset = _itemList.Count * _itemHeight;
    }

    private void SetContentHeight()
    {
        _scrollRect.content.sizeDelta = new Vector2(_scrollRect.content.sizeDelta.x, SpecDataManager.Instance.PassInfoCount * _itemHeight);
    }

    private bool RelocationItem(ItemLumberPass item, float contentY, float scrollHeight)
    {
        if (item.transform.localPosition.y + contentY > (_itemHeight * 2f) - 10f)
        {
            item.transform.localPosition -= new Vector3(0, _offset);
            return true;
        }
        else if (item.transform.localPosition.y + contentY < -scrollHeight - _itemHeight)
        {
            item.transform.localPosition += new Vector3(0, _offset);
            return true;
        }

        return false;
    }
    private void OnScrollChanged()
    {
        RectTransform scrollRect = _scrollRect.GetComponent<RectTransform>();
        float scrollHeight = scrollRect.rect.height;
        float contentY = _scrollRect.content.anchoredPosition.y;

        foreach (var item in _itemList)
        {
            bool changed = RelocationItem(item, contentY, scrollHeight);
            if (changed)
            {
                int idx = (int)(-item.transform.localPosition.y / _itemHeight);
                SetItemData(item, idx + 1);
            }
        }
    }
    private void UpdateCurrency(CurrencyDataType currencyDataType)
    {
        int currency = DataManager.Instance.GetCurrency(currencyDataType);
        switch (currencyDataType)
        {
            case CurrencyDataType.GOLD:
                _gameMoneyText.text = currency.ToString();
                break;
            case CurrencyDataType.GEM:
                _gemText.text = currency.ToString();
                break;
            case CurrencyDataType.FISH:
                _upgradeText.text = currency.ToString();
                break;
            case CurrencyDataType.LEVELUP_POINT:
                _LevelUpPointText.text = currency.ToString();
                break;
        }
    }
    
    private void OnDestroy()
    {
        _scrollRect.onValueChanged.RemoveListener(_ => OnScrollChanged());
        _premiumPassBtn.onClick.RemoveListener(Btn_BuyPremiumPass);
        _onChangedCurrency -= UpdateCurrency;
        
    }
}
