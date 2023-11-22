using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public enum PoolType
{
    BubbleSplash,
    FlyAsYouCanRewardPrefab,
}
public class PoolHandler : MonoBehaviour
{
    public static PoolHandler Instance;
    private Dictionary<PoolType, ObjectPool<Transform>> _pools = new();
    private void Awake()
    {
        Instance = this;
    }
    public ObjectPool<Transform> Create(Transform prefab, PoolType poolType, int initial = 10, int max = 20, bool collectionChecks = false)
    {
        if (_pools.ContainsKey(poolType))
        {
            if (_pools[poolType].Get() == prefab)
                return _pools[poolType];
            else
                _pools[poolType].Dispose();

        }

        var pool = new ObjectPool<Transform>(
            () =>
            {
                return Instantiate(prefab);
            },
            GetSetup,
            ReleaseSetup,
            DestroySetup,
            collectionChecks,
            initial,
            max);

        if (_pools.ContainsKey(poolType)) _pools[poolType] = pool;
        else _pools.Add(poolType, pool);

        return pool;
    }
    public Transform Get(PoolType poolType)
    {
        return _pools[poolType].Get();
    }
    public void Release(Transform prefab, PoolType poolType, float delay = 0f)
    {
        if (delay == 0f) _pools[poolType].Release(prefab);
        else
        {
            var _delay = new WaitForSeconds(delay);
            StartCoroutine(Release(prefab, poolType, _delay));
        }
    }
    private IEnumerator Release(Transform prefab, PoolType poolType, WaitForSeconds delay)
    {
        yield return delay;
        _pools[poolType].Release(prefab);
    }
    protected virtual void GetSetup(Transform obj) => obj.gameObject.SetActive(true);
    protected virtual void ReleaseSetup(Transform obj) => obj.gameObject.SetActive(false);
    protected virtual void DestroySetup(Transform obj) => Destroy(obj.gameObject);
}
