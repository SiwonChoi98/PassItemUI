using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class ResourceManager : Singleton<ResourceManager>
{
    public RewardResourceDatas RewardResourceDatas => _rewardResourceDatas;
    public EtcDatas EtcDatas => _etcDatas;
    public SoundDatas SoundDatas => _soundDatas;
    
    
    private RewardResourceDatas _rewardResourceDatas;
    private const string _rewardResourceDataPath = "RewardResourceData/RewardResourceData";
    
    private EtcDatas _etcDatas;
    private const string _etcDataPath = "EtcData/EtcData";

    private SoundDatas _soundDatas;
    private const string _soundDataPath = "SoundData/SoundData";
    protected override void Awake()
    {
        base.Awake();

        SetRewardDatas();
        SetEtcDatas();
        SetSoundDatas();
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
    
    private void SetSoundDatas()
    {
        SoundDatas resource = Resources.Load<SoundDatas>(_soundDataPath);
        if (resource == null)
        {
            Debug.LogError("SoundData is Null");
            return;
        }
        
        _soundDatas = resource;
    }
}
