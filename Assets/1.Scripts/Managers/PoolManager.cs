using System;
using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PoolManager : Singleton<PoolManager>
{
    [SerializedDictionary("type", "queue")]
    private SerializedDictionary<PoolObjectType, Queue<BasePoolObject>> PoolDictionary = new SerializedDictionary<PoolObjectType, Queue<BasePoolObject>>();
    
    private void OnEnable()
    {
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }
    
    private void OnSceneUnloaded(Scene current)
    {
        ClearAllPools();
    }

    private void ClearAllPools()
    {
        PoolDictionary.Clear();
    }
    
    //public-----------------------------------------------------------------------
    //생성

    public BasePoolObject SpawnUI(PoolObjectType poolObjectType, BasePoolObject basePoolObject, Transform trans)
    {
        BasePoolObject basePoolObject1 = SpawnFromPool(poolObjectType, basePoolObject, trans);
        basePoolObject1.SetPoolObjectType(poolObjectType);

        return basePoolObject1;
    }
    
    //UI
    public BasePoolObject SpawnFromPool(PoolObjectType poolObjectType, BasePoolObject poolObject, Transform spawnTransform)
    {
        if (PoolDictionary.TryGetValue(poolObjectType, out Queue<BasePoolObject> queue))
        {
            if (queue.Count > 0)
            {
                BasePoolObject poolObj = DequeuePoolObject(poolObjectType);
                
                poolObj.transform.SetParent(spawnTransform);
                poolObj.transform.position = spawnTransform.position;
                poolObj.gameObject.SetActive(true);
                
                return poolObj;
            }
            else
            {
                return CreatePoolObject(poolObject, spawnTransform);   
            }
        }
        else
        {       
            return CreatePoolObject(poolObject, spawnTransform);
        }
    }
    
    //해제
    public void ReturnToPool(PoolObjectType poolObjectType, BasePoolObject poolObject)
    {
        poolObject.transform.SetParent(null);
        poolObject.gameObject.SetActive(false);
        
        if (PoolDictionary.ContainsKey(poolObjectType))
        {
            EnqueuePoolObject(poolObjectType, poolObject);
        }
        else
        {
            // 키가 없는 경우 새로운 큐를 만들어 추가
            PoolDictionary[poolObjectType] = new Queue<BasePoolObject>();
            EnqueuePoolObject(poolObjectType, poolObject);
        }
    }

    //해제 위치 지정
    public void ReturnToPool(PoolObjectType poolObjectType, BasePoolObject poolObject, Transform parent)
    {
        poolObject.transform.SetParent(parent);
        poolObject.gameObject.SetActive(false);
        
        if (PoolDictionary.ContainsKey(poolObjectType))
        {
            EnqueuePoolObject(poolObjectType, poolObject);
        }
        else
        {
            // 키가 없는 경우 새로운 큐를 만들어 추가
            PoolDictionary[poolObjectType] = new Queue<BasePoolObject>();
            EnqueuePoolObject(poolObjectType, poolObject);
        }
    }
    
    //private---------------------------------------------------
    private BasePoolObject CreatePoolObject(BasePoolObject poolObject, Transform spawnTransform)
    {
        BasePoolObject obj = Instantiate(poolObject, spawnTransform);
        return obj;
    }
    
    private BasePoolObject DequeuePoolObject(PoolObjectType poolObjectType)
    {
        BasePoolObject obj = PoolDictionary[poolObjectType].Dequeue();
        return obj;
    }
    
    private void EnqueuePoolObject(PoolObjectType poolObjectType, BasePoolObject poolObject)
    {
        PoolDictionary[poolObjectType].Enqueue(poolObject);
    }

}
