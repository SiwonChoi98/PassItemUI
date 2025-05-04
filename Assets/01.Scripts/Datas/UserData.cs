using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

[System.Serializable]
public class UserData
{
    public CurrencyData CurrencyData;
    public PassData PassData;
}

[System.Serializable]
public class CurrencyData
{
    public int GameMoney;
    public int Gem;
    public int Upgrade;
    public int LevelUpPoint;
}

[System.Serializable]
public class PassData
{
    public bool IsSpecialPassEnabled = false;
    
    public int PassLevel;
    public int PassExp;
    
    [JsonProperty]
    public Dictionary<int, bool> RewardsReceivedDic = new Dictionary<int, bool>();
    
    [JsonProperty]
    public Dictionary<int, bool> SpecialRewardsReceivedDic = new Dictionary<int, bool>();
    
    public DateTime LastPassResetTime = DateTime.UtcNow;
}