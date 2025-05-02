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

        SetLumberPassData();
        SpawnItem();
        SetContentHeight();

        _scrollRect.onValueChanged.AddListener(_ => OnScrollChanged());
        _premiumPassBtn.onClick.AddListener(Btn_BuyPremiumPass);

        _onChangedCurrency += UpdateCurrency;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            DataManager.Instance.AddPass(PassDataType.EXP, 50);
            SetPassLevelData();
            SetItemListLockImage();
        }
    }
    
    private void SetLumberPassData()
    {
        UserData userData = DataManager.Instance.UserData;
        
        _gameMoneyText.text = userData.CurrencyData.GameMoney.ToString();
        _gemText.text = userData.CurrencyData.Gem.ToString();
        _upgradeText.text = userData.CurrencyData.Upgrade.ToString();
        _upgradeText.text = userData.CurrencyData.LevelUpPoint.ToString();
        
        if (userData.PassData.IsSpecialPassEnabled)
        {
            _premiumPassBtn.gameObject.SetActive(false);
        }

        SetPassLevelData();
    }

    private void SetPassLevelData()
    {
        UserData userData = DataManager.Instance.UserData;
        
        _passLevelText.text = "Rank : " + userData.PassData.PassLevel;
        
        float userPassExp = userData.PassData.PassExp;
        float userNextLevelNeedExp = SpecDataManager.Instance.GetPassInfoData(userData.PassData.PassLevel).need_exp;
        _passExpText.text = $"<color=#FFFFEB>{userPassExp}</color><color=#FFFFEB>/{userNextLevelNeedExp}</color>";
        _passExpSlider.value = userPassExp / userNextLevelNeedExp;
    }
    private void SetItemData(ItemLumberPass item, int key)
    {
        PassInfoData passInfoData = SpecDataManager.Instance.GetPassInfoData(key);
        
        item.SetData(passInfoData);
        //SetItemLevelLine(passInfoData, item);
        
        item.OnRewardReceived = () => UpdateCurrency((CurrencyDataType)passInfoData.reward_idx);
        item.OnSpecialRewardReceived = () => UpdateCurrency((CurrencyDataType)passInfoData.special_reward_idx);
    }

    /*private void SetItemLevelLine(PassInfoData passInfoData, ItemLumberPass item)
    {
        UserData userData = DataManager.Instance.UserData;
        
        int currentLevel = userData.PassData.PassLevel;
        int currentExp = userData.PassData.PassExp;
    
        if (passInfoData.pass_level == currentLevel)
        {
            int needExp = SpecDataManager.Instance.GetPassInfoData(currentLevel).need_exp;
    
            float t = (float)currentExp / needExp; // 현재 경험치 비율 (0.0 ~ 1.0)
    
            // 예: 이전 0.5 기준으로 레벨 라인을 표시하고 싶다면...
            float from = 0.5f;  // 이전 레벨 절반
            float to = 1f;      // 현재 레벨까지
    
            float fill = Mathf.Lerp(from, to, t); // 레벨 반~전체까지 보간
    
            item.SetLevelLine(fill, 1f);
        }
        else if (passInfoData.pass_level < currentLevel)
        {
            // 이미 지난 레벨이면 항상 가득 찬 상태
            item.SetLevelLine(1f, 1f);
        }
        else
        {
            // 아직 도달하지 않은 레벨이면 0
            item.SetLevelLine(0f, 1f);
        }
    }*/
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

    private void Btn_BuyPremiumPass()
    {
        UserData userData = DataManager.Instance.UserData;
        if (userData.PassData.IsSpecialPassEnabled)
            return;

        DataManager.Instance.BuySpecialPass();
        _premiumPassBtn.gameObject.SetActive(false);

        SetItemListLockImage();
        
        Debug.Log("Buy Premium Pass");
    }

    private void SetItemListLockImage()
    {
        for (int i = 0; i < _itemList.Count; i++)
        {
            _itemList[i].SetLockImage();
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
