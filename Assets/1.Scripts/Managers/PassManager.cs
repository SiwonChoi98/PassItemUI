using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class PassManager : Singleton<PassManager>
{
    private void Start()
    {
        //CheckPassLoop().Forget();
    }

    private async UniTaskVoid CheckPassLoop()
    {
        var token = this.GetCancellationTokenOnDestroy();

        while (!token.IsCancellationRequested)
        {
            CheckPassReset();
            await UniTask.Delay(1000, cancellationToken: token);
        }
    }
    
    /*public void CheckPassReset()
    {
        long currentUnixTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        UserData userData = DataManager.Instance.UserData;
        int passDurationSeconds = SpecDataManager.Instance.GetDefineData("pass_end_time").value;
        
        if (userData.PassData.PassEndUnixTime == 0)
        {
            // 처음 실행 시 초기화 시간 설정
            userData.PassData.PassEndUnixTime = currentUnixTime + passDurationSeconds;
            DataManager.Instance.SaveUserData();
            return;
        }

        if (currentUnixTime >= userData.PassData.PassEndUnixTime)
        {
            Debug.Log("패스 시간 만료됨. 패스 초기화!");

            // 데이터 초기화
            DataManager.Instance.InitPassData(currentUnixTime, passDurationSeconds);
        }
    }*/
    
    private void CheckPassReset()
    {
        long currentUnixTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        if (DataManager.Instance == null)
            return;
        
        var userData = DataManager.Instance.UserData;
        if (userData == null)
        {
            Debug.LogWarning("UserData is null");
            return;
        }

        var defineData = SpecDataManager.Instance.GetDefineData("pass_end_time");
        if (defineData == null)
        {
            Debug.LogWarning("pass_end_time value is null");
            return;
        }

        int passDurationSeconds = defineData.value;

        if (userData.PassData.PassEndUnixTime == 0)
        {
            userData.PassData.PassEndUnixTime = currentUnixTime + passDurationSeconds;
            DataManager.Instance.SaveUserData();
            return;
        }

        if (currentUnixTime >= userData.PassData.PassEndUnixTime)
        {
            Debug.Log("패스 시간 만료됨. 패스 초기화!");
            DataManager.Instance.InitPassData(currentUnixTime, passDurationSeconds);
        }
    }
}
