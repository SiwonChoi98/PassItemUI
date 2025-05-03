using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utills
{
    public static Sprite SetRewardSprite(int rewardIndex)
    {
        if (ResourceManager.Instance == null)
            return null;
        
        if (ResourceManager.Instance.RewardResourceDatas.RewardSpriteDic.TryGetValue((Reward_Type)rewardIndex, out var rewardSprite))
        {
            return rewardSprite;
        }

        Debug.LogError("RewardSprite is Null");
        return null;
    }
}
