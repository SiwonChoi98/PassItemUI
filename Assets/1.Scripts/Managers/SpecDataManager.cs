using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;
public partial class SpecDataManager
{
    private const string specDefineDataPath = "SpecData/Define";
    private const string specRewardTypeDataPath = "SpecData/RewardType";
    private const string specPassInfoDataPath = "SpecData/Pass_Info";
    
    [SerializedDictionary("key", "data")]
    private SerializedDictionary<string, DefineData> _defineDataDic = new SerializedDictionary<string, DefineData>();
    [SerializedDictionary("key", "data")]
    private SerializedDictionary<string, RewardTypeData> _rewardTypeDataDic = new SerializedDictionary<string, RewardTypeData>();
    [SerializedDictionary("key", "data")]
    private SerializedDictionary<int, PassInfoData> _passInfoDataDic = new SerializedDictionary<int, PassInfoData>();
    
    public DefineData GetDefineData(string key)
    {
        if (_defineDataDic.TryGetValue(key, out var data))
            return data;

        Debug.LogWarning($"[DefineData] Key not found: {key}");
        return null;
    }

    public RewardTypeData GetRewardTypeData(string key)
    {
        if (_rewardTypeDataDic.TryGetValue(key, out var data))
            return data;

        Debug.LogWarning($"[RewardTypeData] Key not found: {key}");
        return null;
    }

    public PassInfoData GetPassInfoData(int id)
    {
        if (_passInfoDataDic.TryGetValue(id, out var data))
            return data;

        Debug.LogWarning($"[PassInfoData] ID not found: {id}");
        return null;
    }
}

public partial class SpecDataManager : Singleton<SpecDataManager>
{
    protected override void Awake()
    {
        base.Awake();

        LoadSpecDataAll();
    }
    
    
    //스펙 데이터 로드
    public void LoadSpecDataAll()
    {
        LoadDefineData();
        LoadRewardTypeData();
        LoadPassInfoData();
    }
    
    private void LoadDefineData()
    {
        TextAsset text = Resources.Load<TextAsset>(specDefineDataPath);
        if (text == null) { Debug.LogError("DefineData.json not found!"); return; }

        List<DefineData> list = JsonUtilityWrapper.FromJsonList<DefineData>(text.text);
        foreach (var data in list)
        {
            _defineDataDic[data.index] = data;
        }
    }

    private void LoadRewardTypeData()
    {
        TextAsset text = Resources.Load<TextAsset>(specRewardTypeDataPath);
        if (text == null) { Debug.LogError("RewardTypeData.json not found!"); return; }

        List<RewardTypeData> list = JsonUtilityWrapper.FromJsonList<RewardTypeData>(text.text);
        foreach (var data in list)
        {
            string key = $"{data.reward_type}_{data.reward_idx}";
            _rewardTypeDataDic[key] = data;
        }
    }

    private void LoadPassInfoData()
    {
        TextAsset text = Resources.Load<TextAsset>(specPassInfoDataPath);
        if (text == null) { Debug.LogError("PassInfoData.json not found!"); return; }

        List<PassInfoData> list = JsonUtilityWrapper.FromJsonList<PassInfoData>(text.text);
        foreach (var data in list)
        {
            _passInfoDataDic[data.pass_level] = data;
        }
    }
}


