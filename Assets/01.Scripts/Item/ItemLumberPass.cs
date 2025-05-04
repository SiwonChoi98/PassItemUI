using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class ItemLumberPass : MonoBehaviour
{
    private PassInfoData _data;
    public PassInfoData Data => _data;
    
    [Header("Level")] 
    [SerializeField] private Text _disabledLevelText;
    [SerializeField] private Text _enabledLevelText;
    [SerializeField] private Image _levelLineEnabledImage;
    [SerializeField] private GameObject _currentLevelEnabled;
    
    [SerializeField] private Transform _levelEffectParent;
    
    [Header("Reward")]
    [SerializeField] private Text _rewardValueText;
    [SerializeField] private Image _rewardImage;
    [SerializeField] private Image _rewardRockImage;
    [SerializeField] private Image _rewardCheckImage;
    
    [Header("Special Reward")]
    [SerializeField] private Text _specialRewardValueText;
    [SerializeField] private Image _specialRewardImage;
    [SerializeField] private Image _specialRewardRockImage;
    [SerializeField] private Image _specialrewardCheckImage;

    [Header("RewardReceive")] 
    public Action OnRewardReceived;
    public Action OnSpecialRewardReceived;
    
    public Action<PassInfoData, PassDataType, RectTransform> OnRewardSpawn;

    [Header("Noti")] 
    public Action<NotificationType> OnNotiSpawn;
    
    public void Btn_RewardReceive()
    {
        bool isReceived = DataManager.Instance.IsRewardReceived(PassDataType.REWARD_RECEIVED, _data.pass_level);
        if (isReceived)
        {
            OnNotiSpawn?.Invoke(NotificationType.IS_RECEIVED);
            SoundManager.Instance.Play_SFX(SoundType.NOTIPOPUP_SFX, 0.5f);
            return;
        }
            
        
        if (_data.pass_level > DataManager.Instance.UserData.PassData.PassLevel)
        {
            OnNotiSpawn?.Invoke(NotificationType.LEVEL_NOT_ENOUGH);
            SoundManager.Instance.Play_SFX(SoundType.NOTIPOPUP_SFX, 0.5f);
            return;
        }
        
        //data
        DataManager.Instance.ClaimReward(PassDataType.REWARD_RECEIVED, _data.pass_level);
        DataManager.Instance.AddCurrency((CurrencyDataType)_data.reward_idx, _data.reward_value);
        
        SoundManager.Instance.Play_SFX(SoundType.ADDCOIN_SFX, 0.3f);
        
        //ui
        SetCheckImage();
        OnRewardReceived?.Invoke();

        OnRewardSpawn?.Invoke(_data, PassDataType.REWARD_RECEIVED, GetComponent<RectTransform>());
        
        SoundManager.Instance.Play_SFX(SoundType.BUTTON_SFX, 0.3f);
        Debug.Log("rewardReceive : " + _data.pass_level);
    }

    public void Btn_SpecialRewardReceive()
    {
        bool isReceived = DataManager.Instance.IsRewardReceived(PassDataType.SPECIALREWARD_RECEIVED, _data.pass_level);
        if (isReceived)
        {
            OnNotiSpawn?.Invoke(NotificationType.IS_RECEIVED);
            SoundManager.Instance.Play_SFX(SoundType.NOTIPOPUP_SFX, 0.5f);
            return;
        }


        if (!DataManager.Instance.UserData.PassData.IsSpecialPassEnabled)
        {
            OnNotiSpawn?.Invoke(NotificationType.SPECIALPASS_NOT_ENABLED);
            SoundManager.Instance.Play_SFX(SoundType.NOTIPOPUP_SFX, 0.5f);
            return;
        }
            
        
        if (_data.pass_level > DataManager.Instance.UserData.PassData.PassLevel)
        {
            OnNotiSpawn?.Invoke(NotificationType.LEVEL_NOT_ENOUGH);
            SoundManager.Instance.Play_SFX(SoundType.NOTIPOPUP_SFX, 0.5f);
            return;
        }
        
        //data
        DataManager.Instance.ClaimReward(PassDataType.SPECIALREWARD_RECEIVED, _data.pass_level);
        DataManager.Instance.AddCurrency((CurrencyDataType)_data.special_reward_idx, _data.special_reward_value);
        
        SoundManager.Instance.Play_SFX(SoundType.ADDCOIN_SFX, 0.3f);
        
        //ui
        SetCheckImage();
        OnSpecialRewardReceived?.Invoke();
        
        OnRewardSpawn?.Invoke(_data, PassDataType.SPECIALREWARD_RECEIVED, GetComponent<RectTransform>());
        
        SoundManager.Instance.Play_SFX(SoundType.BUTTON_SFX, 0.3f);
        Debug.Log("specialRewardReceive : " + _data.pass_level);
    }
    public void SetData(PassInfoData data)
    {
        _data = data;
        
        _disabledLevelText.text = _data.pass_level.ToString();
        _enabledLevelText.text = _data.pass_level.ToString();
        
        _rewardValueText.text = _data.reward_value.ToString();
        _specialRewardValueText.text = _data.special_reward_value.ToString();

        _rewardImage.sprite = Utills.SetRewardSprite(_data.reward_idx);
        _specialRewardImage.sprite = Utills.SetRewardSprite(_data.special_reward_idx);

        SetLockImage();
        SetCheckImage();
        SetLineLevelImage();
        SetLevelLine();
    }
    
    public void SetLineLevelImage()
    {
        UserData userData = DataManager.Instance.UserData;

        bool isLineLevelEnable = userData.PassData.PassLevel >= _data.pass_level;
        _currentLevelEnabled.SetActive(isLineLevelEnable);
    }
    public void SetCheckImage()
    {
        bool isRewardCheckImage = DataManager.Instance.IsRewardReceived(PassDataType.REWARD_RECEIVED, _data.pass_level);
        _rewardCheckImage.gameObject.SetActive(isRewardCheckImage);
        
        bool isSpecialRewardCheckImage = DataManager.Instance.IsRewardReceived(PassDataType.SPECIALREWARD_RECEIVED, _data.pass_level);
        _specialrewardCheckImage.gameObject.SetActive(isSpecialRewardCheckImage);
    }
    
    public void SetLockImage()
    {
        if(DataManager.Instance == null)
            return;
        
        UserData userData = DataManager.Instance.UserData;
        
        bool isRockShow = userData.PassData.PassLevel < _data.pass_level;
        _rewardRockImage.gameObject.SetActive(isRockShow);

        bool isSpecialRockShow =
            (userData.PassData.PassLevel < _data.pass_level) || userData.PassData.IsSpecialPassEnabled == false;
        
        _specialRewardRockImage.gameObject.SetActive(isSpecialRockShow);
    }

    public void SetLevelLine()
    {
        UserData userData = DataManager.Instance.UserData;
        int userLevel = userData.PassData.PassLevel;

        float userExp = userData.PassData.PassExp;
        
        
        // 같은 레벨: 0.5 + 일부 경험치
        if(userLevel == _data.pass_level)
        {
            float needExp = _data.need_exp;
            float remainExp = Mathf.Min(Mathf.Clamp01(userExp / needExp), 0.5f);
            
            SetLevelLineFill(0.5f + remainExp, 1);
            return;
        }
        
        // 1레벨 낮을 때: 0 ~ 0.5 사이로 경험치
        if (userLevel < _data.pass_level && userLevel == _data.pass_level-1)
        {
            float needExp = SpecDataManager.Instance.GetPassInfoData(userLevel).need_exp;
            float remainExp = Mathf.Max((Mathf.Clamp01((userExp / needExp) - 0.5f)), 0);
            SetLevelLineFill(remainExp, 1);
            return;
        }
        
        
        // 더 높은 레벨: 전체 게이지 채움
        if (userLevel > _data.pass_level)
        {
            SetLevelLineFill(1, 1);
            return;
        }

        // 2레벨 이상 낮음: 게이지 없음
        if (userLevel < _data.pass_level)
        {
            SetLevelLineFill(0, 1);
            return;
        }
    }

    public Transform GetLevelEffectParent() => _levelEffectParent;
    
    private void SetLevelLineFill(float current, float max)
    {
        _levelLineEnabledImage.fillAmount = current / max;
    }
}
