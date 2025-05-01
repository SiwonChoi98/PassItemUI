using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

public partial class DataManager
{
    
}

public partial class DataManager : Singleton<DataManager>
{
    private string _userDataPath;

    // 유저 데이터 객체
    public UserData UserData { get; private set; }

    protected override void Awake()
    {
        _userDataPath = Path.Combine(Application.persistentDataPath, "userData.json");

        //삭제
        //DeleteUserData();
        
        //로드
        LoadUserDataAsync().Forget();
    }

    
    //초기화
    private void InitializeUserData()
    {
        UserData = new UserData
        {
            CurrencyData = new CurrencyData(),
            PassData = new PassData()
        };
        
        SaveUserDataAsync().Forget();
    }

    //유저 데이터 저장
    [Obsolete("Obsolete")]
    public async UniTask SaveUserDataAsync()
    {
        string jsonData = JsonConvert.SerializeObject(UserData, Formatting.Indented);
        
        // 비동기로 파일 쓰기
        await UniTask.Run(() => File.WriteAllText(_userDataPath, jsonData));
        Debug.Log("User Data Saved");
    }

    //유저 데이터 로드
    [Obsolete("Obsolete")]
    public async UniTask LoadUserDataAsync()
    {
        if (File.Exists(_userDataPath))
        {
            string jsonData = await UniTask.Run(() => File.ReadAllText(_userDataPath));
            
            UserData = JsonConvert.DeserializeObject<UserData>(jsonData);
            Debug.Log("User data loaded.");
        }
        else
        {
            Debug.LogWarning("Init DefaultData");
            InitializeUserData();
        }
    }

    //유저 데이터 삭제
    [Obsolete("Obsolete")]
    public async UniTask DeleteUserDataAsync()
    {
        if (File.Exists(_userDataPath))
        {
            // 비동기로 파일 삭제
            await UniTask.Run(() => File.Delete(_userDataPath));
            UserData = null;  // UserData 초기화
            Debug.Log("User data deleted.");
        }
        else
        {
            Debug.LogWarning("No user data found to delete.");
        }
    }
}
