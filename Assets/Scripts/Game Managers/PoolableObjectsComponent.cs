using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolableObjectsComponent<T>  where T : MonoBehaviour, IPoolableObject
{
    [SerializeField]
    private GameObject _prefab;

    Queue<T> _pool = new Queue<T>();

    private static Vector2 POS_OUTSIDE_OF_PLAY = new Vector2(100000, 10000);

    public void Initialize(GameObject prefab)
    {
        _prefab = prefab;
    }

    public bool IsInitialized()
    {
        return _prefab;
    }

    public void AddObjectToPool(T poolableObject)
    {
        _pool.Enqueue(poolableObject);
        poolableObject.ActivateObject(false);
    }

    public T GetPoolableObject()
    {
        if (_pool.Count == 0)
        {
            SpawnNewObjectIntoPool();
        }

        T pooledObject = _pool.Dequeue();
        pooledObject.ActivateObject(true);
        return pooledObject;
    }

    private void SpawnNewObjectIntoPool()
    {
        if (_prefab)
        {
            T poolableObject = Object.Instantiate(_prefab, POS_OUTSIDE_OF_PLAY, Quaternion.identity).GetComponent<T>();
            if (poolableObject)
            {
                _pool.Enqueue(poolableObject);
            }
            else
            {
                Debug.LogError("Pool with " + _prefab.name + " is null");
            }
        }
        else
        {
            Debug.LogError("Prefab needed for object pool is null");
        }
    }
}
