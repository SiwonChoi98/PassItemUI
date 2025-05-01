using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemLumberPass : MonoBehaviour
{
    [SerializeField] private Text _levelText;
    
    [SerializeField] private Text _rewardValueText;
    [SerializeField] private Text _specialRewardValueText;

    [SerializeField] private Image _rewardImage;
    [SerializeField] private Image _specialRewardImage;
    
    public void SetData(PassInfoData data)
    {
        _levelText.text = data.pass_level.ToString();
        _rewardValueText.text = data.reward_value.ToString();
        _specialRewardValueText.text = data.special_reward_value.ToString();

        _rewardImage.sprite = SetRewardSprite(data.reward_idx);
        _specialRewardImage.sprite = SetRewardSprite(data.special_reward_idx);
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
    
    //레벨 게이지 
}
