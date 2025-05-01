using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class ResourceManager : Singleton<ResourceManager>
{
    private RewardResourceDatas _rewardResourceDatas;
    private const string _rewardResourceDataPath = "RewardResourceData/RewardResourceData";

    public RewardResourceDatas RewardResourceDatas => _rewardResourceDatas;
    
    protected override void Awake()
    {
        base.Awake();

        SetRewardDatas();
    }

    private void SetRewardDatas()
    {
        RewardResourceDatas resource = Resources.Load<RewardResourceDatas>(_rewardResourceDataPath);
        if (resource == null)
        {
            Debug.LogError("RewardResourceData is Null");
            return;
        }
        _rewardResourceDatas = resource;
    }
}
