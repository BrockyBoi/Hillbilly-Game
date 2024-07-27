using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Weaponry;

public enum ObjectPoolTypes
{
    XPOrb, Enemy, Projectile
}

public class PoolableObjectsManager : MonoBehaviour
{
    public static PoolableObjectsManager Instance { get; private set; }

    [Title("Prefabs")]
    [SerializeField]
    private GameObject _xpOrbPrefab;

    [SerializeField]
    private GameObject _enemyPrefab;

    [SerializeField]
    private GameObject _projectilePrefab;

    Dictionary<ObjectPoolTypes, Queue<IPoolableObject>> _pools = new Dictionary<ObjectPoolTypes, Queue<IPoolableObject>>();

    private static Vector2 POS_OUTSIDE_OF_PLAY = new Vector2(100000, 10000);

    private void Awake()
    {
        Instance = this;
    }

    public void AddObjectToPool(IPoolableObject poolableObject, ObjectPoolTypes poolType)
    {
        if (!_pools.ContainsKey(poolType))
        {
            _pools.Add(poolType, new Queue<IPoolableObject>());
        }

        _pools[poolType].Enqueue(poolableObject);
        poolableObject.ActivateObject(false);
    }

    public IPoolableObject GetPoolableObject(ObjectPoolTypes poolType)
    {
        if (!_pools.ContainsKey(poolType))
        {
            _pools.Add(poolType, new Queue<IPoolableObject>());
        }

        if (_pools[poolType].Count == 0)
        {
            SpawnNewObjectIntoPool(poolType);
        }

        return _pools[poolType].Dequeue();
    }

    private void SpawnNewObjectIntoPool(ObjectPoolTypes objectPoolType)
    {
        switch (objectPoolType)
        {
            case ObjectPoolTypes.XPOrb:
                {
                    XPOrb orb = Instantiate(_xpOrbPrefab, POS_OUTSIDE_OF_PLAY, Quaternion.identity).GetComponent<XPOrb>();
                    _pools[objectPoolType].Enqueue(orb);
                    break;
                }

            case ObjectPoolTypes.Enemy:
                {
                    Enemy enemy = Instantiate(_enemyPrefab, POS_OUTSIDE_OF_PLAY, Quaternion.identity).GetComponent<Enemy>();
                    _pools[objectPoolType].Enqueue(enemy);
                    break;
                }

            case ObjectPoolTypes.Projectile:
                {
                    Projectile projectile = Instantiate(_projectilePrefab, POS_OUTSIDE_OF_PLAY, Quaternion.identity).GetComponent<Projectile>();
                    _pools[objectPoolType].Enqueue(projectile);
                    break;
                }
        }
    }
}
