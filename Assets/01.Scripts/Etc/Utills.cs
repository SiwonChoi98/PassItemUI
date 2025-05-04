using System;
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

    public static string GetNotiPopupText(NotificationType notificationType)
    {
        switch (notificationType)
        {
            case NotificationType.LEVEL_NOT_ENOUGH:
                return "레벨이 부족하여 보상을 수령할 수 없습니다!";
            case NotificationType.SPECIALPASS_NOT_ENABLED:
                return "스페셜 패스를 구매해주세요!";
            case NotificationType.IS_RECEIVED:
                return "이미 보상을 수령하였습니다!";
        }

        return null;
    }
    
    public static DateTime SafeSubtract(DateTime time, TimeSpan span)
    {
        if (time.Ticks > span.Ticks)
            return time - span;
        else
            return DateTime.UtcNow;
    }
}
