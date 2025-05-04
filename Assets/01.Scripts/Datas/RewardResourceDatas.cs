using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;

[CreateAssetMenu(fileName = "RewardResourceData", menuName = "Scriptable Object Asset/RewardResourceData")]
public class RewardResourceDatas : ScriptableObject
{
    [SerializedDictionary("RewardType", "Sprite")]
    public SerializedDictionary<Reward_Type, Sprite> RewardSpriteDic;

    public RewardObj RewardObj;
    public BasePoolObject OpenObj;
}
