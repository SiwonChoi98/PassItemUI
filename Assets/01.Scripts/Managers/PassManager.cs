using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class PassManager : Singleton<PassManager>
{
    [Header("PassTime")]
    private TimeSpan _cycle;
    private bool _isCycleValid = false;
    
    private DateTime _fixedStartTime = new DateTime(2025, 5, 3, 16, 20, 0, DateTimeKind.Utc);
    private DateTime _nextResetTime;

    public Action<DateTime> OnChangedPassTime;
    
    [Header("PassExp")]
    //10초마다 올라가는 경험치 값
    [SerializeField] private float _userPassExpUpTime;
    private bool _isActive = false;
    
    private async void Start()
    {
        await UniTask.WaitUntil(() => DataManager.Instance != null && DataManager.Instance.UserData != null);
        
        SetCycleTime();
        
        UpdateNextResetTime();
        
        UpdateCycleRoutine().Forget();
        
        _isActive = true;
        AutoIncreaseExp().Forget();
    }
    private void OnDestroy()
    {
        _isActive = false;
    }
    
    private void SetCycleTime()
    {
        int cycleTime = SpecDataManager.Instance.GetDefineData("pass_end_time").value;
        if (cycleTime == 0)
        {
            Debug.LogError("SpecData is Null");
            _isCycleValid = false;
            return;
        }
        
        _cycle = TimeSpan.FromSeconds(cycleTime);
        _isCycleValid = true;
    }
    
    private async UniTaskVoid UpdateCycleRoutine()
    {
        while (true)
        {
            CheckReset();
            UpdateRemainingTimeUI();
            await UniTask.Delay(1000); // 1초마다 업데이트
        }
    }

    private void UpdateNextResetTime()
    {
        if (!_isCycleValid)
            return;
        
        TimeSpan elapsed = DateTime.UtcNow - _fixedStartTime;
        int cyclesPassed = Mathf.FloorToInt((float)(elapsed.TotalSeconds / _cycle.TotalSeconds));
        _nextResetTime = _fixedStartTime.AddSeconds((cyclesPassed + 1) * _cycle.TotalSeconds);
    }

    private void CheckReset()
    {
        if (!_isCycleValid)
            return;
        
        if (DateTime.UtcNow >= _nextResetTime)
        {
            ResetPassData();
            UpdateNextResetTime(); // 다음 초기화 시점 갱신
        }
    }

    private void UpdateRemainingTimeUI()
    {
        if (!_isCycleValid)
            return;

        OnChangedPassTime?.Invoke(_nextResetTime);
    }

    private void ResetPassData()
    {
        DataManager.Instance.InitPassData();
        Debug.Log("패스 데이터가 초기화되었습니다.");
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
