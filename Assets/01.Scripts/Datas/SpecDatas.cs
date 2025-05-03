using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DefineData
{
    public string index;
    public int value;
}

[System.Serializable]
public class RewardTypeData
{
    public int reward_type;
    public int reward_idx;
    public string icon;
}

[System.Serializable]
public class PassInfoData
{
    public int pass_level;
    public int need_exp;
    public int reward_type;
    public int reward_idx;
    public int special_reward_type;
    public int special_reward_idx;
    public int reward_value;
    public int special_reward_value;
}