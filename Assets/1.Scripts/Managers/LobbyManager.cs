using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class LobbyManager : Singleton<LobbyManager>
{
    //10초마다 올라가는 경험치 값
    [SerializeField] private float _userPassExpUpTime;
    private bool _isActive = false;

    private void Start()
    {
        _isActive = true;
        AutoIncreaseExp().Forget();
    }

    private void OnDestroy()
    {
        _isActive = false;
    }

    private async UniTaskVoid AutoIncreaseExp()
    {
        while (_isActive)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(_userPassExpUpTime), cancellationToken: this.GetCancellationTokenOnDestroy());

            int exp = SpecDataManager.Instance.GetDefineData("increase_time_exp").value;
            DataManager.Instance.AddPass(PassDataType.EXP, exp);
            Debug.Log("경험치 증가!! : " + exp);
        }
    }
    
}
