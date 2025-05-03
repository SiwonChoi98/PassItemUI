using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class ResourceManager : Singleton<ResourceManager>
{
    public RewardResourceDatas RewardResourceDatas => _rewardResourceDatas;
    public EtcDatas EtcDatas => _etcDatas;
    
    
    private RewardResourceDatas _rewardResourceDatas;
    private const string _rewardResourceDataPath = "RewardResourceData/RewardResourceData";
    
    private EtcDatas _etcDatas;
    private const string _etcDataPath = "EtcData/EtcData";
    
    protected override void Awake()
    {
        base.Awake();

        SetRewardDatas();
        SetEtcDatas();
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
    
    private void SetEtcDatas()
    {
        EtcDatas resource = Resources.Load<EtcDatas>(_etcDataPath);
        if (resource == null)
        {
            Debug.LogError("EtcData is Null");
            return;
        }
        
        _etcDatas = resource;
    }
}
