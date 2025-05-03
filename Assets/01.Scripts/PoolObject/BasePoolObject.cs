using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePoolObject : MonoBehaviour
{
    [SerializeField] private PoolObjectType _poolObjectType;
    
    public void SetPoolObjectType(PoolObjectType poolObjectType){
        _poolObjectType = poolObjectType;
    }

    public PoolObjectType GetPoolObjectType()
    {
        return _poolObjectType;
    }
    public void ReturnToPool()
    {
        
        if(!gameObject.activeSelf)
            return;
        
        
        PoolManager.Instance.ReturnToPool(_poolObjectType, this);
    }
}