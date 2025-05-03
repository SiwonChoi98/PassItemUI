using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Reward_Type
{
    GOLD = 1,
    GEM = 2,
    FISH = 3,
    LEVELUP_POINT = 4,
}

public enum PoolObjectType
{
    CURRENCYOBJ_UI = 1001, 
    NOTIPOPUP_UI = 1002,
}

public enum NotificationType
{
    LEVEL_NOT_ENOUGH,   
    SPECIALPASS_NOT_ENABLED,
    IS_RECEIVED,
}