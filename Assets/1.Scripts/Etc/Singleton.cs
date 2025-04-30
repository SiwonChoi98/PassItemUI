using System;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    [SerializeField] private bool _isDontDestroy;
    protected bool IsDonDestroy
    {
        get => _isDontDestroy;
        set => _isDontDestroy = value;
    }
    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                Debug.LogWarning($"[Singleton<{typeof(T)}>] Instance is null. Make Instance");
            }
            return instance;
        }
    }

    protected virtual void Awake()
    {
        if (instance == null)
        {
            instance = this as T;

            if (_isDontDestroy)
                DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
}