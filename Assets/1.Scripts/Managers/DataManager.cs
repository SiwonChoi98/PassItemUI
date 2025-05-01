using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

public enum PassDataType
{
    LEVEL,
    EXP,
    
    SPECIALPASS_ENALBED,
    REWARD_RECEIVED,
    SPECIALREWARD_RECEIVED,
}

public enum CurrencyDataType
{
    GOLD = 1,
    GEM = 2,
    FISH = 3,
    LEVELUP_POINT = 4,
}
public partial class DataManager
{
    public UserData UserData => _userData;
    
    private UserData _userData;
    public void AddCurrency(CurrencyDataType currencyDataType, int amount)
    {
        switch (currencyDataType)
        {
            case CurrencyDataType.GOLD:
                AddGold(amount);
                break;
            case CurrencyDataType.GEM:
                AddGem(amount);
                break;
            case CurrencyDataType.FISH:
                AddFish(amount);
                break;
            case CurrencyDataType.LEVELUP_POINT:
                AddLevelUpPoint(amount);
                break;
        }

        SaveUserDataAsync().Forget();
    }
    
    public int GetCurrency(CurrencyDataType currencyDataType)
    {
        switch (currencyDataType)
        {
            case CurrencyDataType.GOLD:
                return _userData.CurrencyData.GameMoney;
            case CurrencyDataType.GEM:
                return _userData.CurrencyData.Gem;
            case CurrencyDataType.FISH:
                return _userData.CurrencyData.Upgrade;
            case CurrencyDataType.LEVELUP_POINT:
                return _userData.CurrencyData.LevelUpPoint;
        }
        return 0;
    }

    public void AddPass(PassDataType passDataType, int amount)
    {
        switch (passDataType)
        {
            case PassDataType.LEVEL:
                AddPassLevel(amount);
                break;
            case PassDataType.EXP:
                AddPassExp(amount);
                break;
        }
        SaveUserDataAsync().Forget();
    }

    public void BuySpecialPass()
    {
        if (_userData.PassData.IsSpecialPassEnabled)
            return;
        
        _userData.PassData.IsSpecialPassEnabled = true;
        SaveUserDataAsync().Forget();
    }
    public void ClaimReward(PassDataType type, int level)
    {
        switch (type)
        {
            case PassDataType.REWARD_RECEIVED:
                if (!_userData.PassData.RewardsReceivedDic.ContainsKey(level))
                {
                    _userData.PassData.RewardsReceivedDic.Add(level, true);
                }
                break;

            case PassDataType.SPECIALREWARD_RECEIVED:
                if (!_userData.PassData.SpecialRewardsReceivedDic.ContainsKey(level))
                {
                    _userData.PassData.SpecialRewardsReceivedDic.Add(level, true);
                }
                break;

            default:
                break;
        }
        SaveUserDataAsync().Forget();
    }
    
    public bool IsRewardReceived(PassDataType type, int rewardId)
    {
        switch (type)
        {
            case PassDataType.REWARD_RECEIVED:
                return _userData.PassData.RewardsReceivedDic.ContainsKey(rewardId) &&
                       _userData.PassData.RewardsReceivedDic[rewardId];
        
            case PassDataType.SPECIALREWARD_RECEIVED:
                return _userData.PassData.SpecialRewardsReceivedDic.ContainsKey(rewardId) &&
                       _userData.PassData.SpecialRewardsReceivedDic[rewardId];
        
            default:
                return false;
        }
    }

    private void AddPassLevel(int amount)
    {
        _userData.PassData.PassLevel += amount;
    }

    private void AddPassExp(int amount)
    {
        _userData.PassData.PassExp += amount;
    }
    private void AddGold(int amount)
    {
        _userData.CurrencyData.GameMoney += amount;
    }
    private void AddGem(int amount)
    {
        _userData.CurrencyData.Gem += amount;
    }
    private void AddFish(int amount)
    {
        _userData.CurrencyData.Upgrade += amount;
    }
    private void AddLevelUpPoint(int amount)
    {
        _userData.CurrencyData.LevelUpPoint += amount;
    }
}

public partial class DataManager : Singleton<DataManager>
{
    private string _userDataPath;
    
    protected override void Awake()
    {
        _userDataPath = Path.Combine(Application.persistentDataPath, "userData.json");

        //삭제
        //DeleteUserDataAsync().Forget();
        
        //로드
        LoadUserDataAsync().Forget();
    }

    
    //초기화
    private void InitializeUserData()
    {
        _userData = new UserData
        {
            CurrencyData = new CurrencyData(),
            PassData = new PassData()
            {
                IsSpecialPassEnabled = false,
                PassExp = 0,
                PassLevel = 1,
            }
        };
        
        SaveUserDataAsync().Forget();
    }

    //유저 데이터 저장
    public async UniTask SaveUserDataAsync()
    {
        string jsonData = JsonConvert.SerializeObject(_userData, Formatting.Indented);
        
        // 비동기로 파일 쓰기
        await UniTask.Run(() => File.WriteAllText(_userDataPath, jsonData));
        Debug.Log("User Data Saved");
    }

    //유저 데이터 로드
    public async UniTask LoadUserDataAsync()
    {
        if (File.Exists(_userDataPath))
        {
            string jsonData = await UniTask.Run(() => File.ReadAllText(_userDataPath));
            
            _userData = JsonConvert.DeserializeObject<UserData>(jsonData);
            Debug.Log("User data loaded.");
        }
        else
        {
            Debug.LogWarning("Init DefaultData");
            InitializeUserData();
        }
    }

    //유저 데이터 삭제
    public async UniTask DeleteUserDataAsync()
    {
        if (File.Exists(_userDataPath))
        {
            // 비동기로 파일 삭제
            await UniTask.Run(() => File.Delete(_userDataPath));
            _userData = null;  // UserData 초기화
            Debug.Log("User data deleted.");
        }
        else
        {
            Debug.LogWarning("No user data found to delete.");
        }
    }
}
