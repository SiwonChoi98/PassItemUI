using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemLumberPass : MonoBehaviour
{
    private PassInfoData _data;
    
    [Header("Level")] 
    [SerializeField] private Text _disabledLevelText;
    [SerializeField] private Text _enabledLevelText;
    [SerializeField] private Image _levelLineDisabledImage;
    [SerializeField] private Image _levelLineEnabledImage;
    [SerializeField] private GameObject _currentLevelEnabled;
    
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
    public void Btn_RewardReceive()
    {
        if (_data.pass_level > DataManager.Instance.UserData.PassData.PassLevel)
        {
            Debug.Log("레벨이 부족합니다.");
            return;
        }
        
        //data
        DataManager.Instance.ClaimReward(PassDataType.REWARD_RECEIVED, _data.pass_level);
        DataManager.Instance.AddCurrency((CurrencyDataType)_data.reward_idx, _data.reward_value);
        //ui
        OnRewardReceived?.Invoke();

        Debug.Log("rewardReceive : " + _data.pass_level);
    }

    public void Btn_SpecialRewardReceive()
    {
        if (_data.pass_level > DataManager.Instance.UserData.PassData.PassLevel)
        {
            Debug.Log("레벨이 부족합니다.");
            return;
        }
        
        //data
        DataManager.Instance.ClaimReward(PassDataType.SPECIALREWARD_RECEIVED, _data.pass_level);
        DataManager.Instance.AddCurrency((CurrencyDataType)_data.special_reward_idx, _data.special_reward_value);
        //ui
        OnSpecialRewardReceived?.Invoke();
        
        Debug.Log("specialRewardReceive : " + _data.pass_level);
    }
    public void SetData(PassInfoData data)
    {
        _data = data;
        
        _disabledLevelText.text = _data.pass_level.ToString();
        _enabledLevelText.text = _data.pass_level.ToString();
        
        _rewardValueText.text = _data.reward_value.ToString();
        _specialRewardValueText.text = _data.special_reward_value.ToString();

        _rewardImage.sprite = SetRewardSprite(_data.reward_idx);
        _specialRewardImage.sprite = SetRewardSprite(_data.special_reward_idx);

        SetLockImage(_data);
        SetCheckImage(_data);
    }

    private Sprite SetRewardSprite(int rewardIndex)
    {
        if (ResourceManager.Instance.RewardResourceDatas.RewardSpriteDic.TryGetValue((Reward_Type)rewardIndex, out var rewardSprite))
        {
            return rewardSprite;
        }

        Debug.LogError("RewardSprite is Null");
        return null;
    }

    private void SetLockImage(PassInfoData data)
    {
        if(DataManager.Instance == null)
            return;
        
        UserData userData = DataManager.Instance.UserData;
        
        bool isRockShow = userData.PassData.PassLevel < data.pass_level;
        _rewardRockImage.gameObject.SetActive(isRockShow);

        bool isSpecialRockShow =
            (userData.PassData.PassLevel < data.pass_level) || userData.PassData.IsSpecialPassEnabled == false;
        
        _specialRewardRockImage.gameObject.SetActive(isSpecialRockShow);
    }

    private void SetCheckImage(PassInfoData data)
    {
        bool isRewardCheckImage = DataManager.Instance.IsRewardReceived(PassDataType.REWARD_RECEIVED, data.pass_level);
        _rewardCheckImage.gameObject.SetActive(isRewardCheckImage);
        
        bool isSpecialRewardCheckImage = DataManager.Instance.IsRewardReceived(PassDataType.SPECIALREWARD_RECEIVED, data.pass_level);
        _specialrewardCheckImage.gameObject.SetActive(isSpecialRewardCheckImage);
    }
    
}
