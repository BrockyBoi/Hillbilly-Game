using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnerController : MonoBehaviour
{
    public static EnemySpawnerController Instance { get; private set; }

    private float _currentSpawnFrequency;

    [Title("Spawn Parameters")]
    [SerializeField]
    private float _maxSpawnRate = .5f;

    [SerializeField]
    private float _startingSpawnFrequency = 2.5f;

    private float _nextSpawnInterval = -1;

    [Title("Spawn Location")]
    [SerializeField]
    private float _minDistanceSpawnedFromPlayer = 25;

    [SerializeField]
    private float _maxDistanceSpawnedFromPlayer = 50;

    WaitForSeconds _waitToSpawnEnemy;

    private List<Enemy> _enemiesInGame = new List<Enemy>();
    public List<Enemy> EnemiesInGame { get { return _enemiesInGame; } }

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        _currentSpawnFrequency = _startingSpawnFrequency;

        _waitToSpawnEnemy = new WaitForSeconds(_currentSpawnFrequency);


        StartMatch();
    }

    // Update is called once per frame
    void Update()
    {


    }

    public void SetSpawnInterval(float spawnInterval)
    {
        _nextSpawnInterval = Mathf.Clamp(spawnInterval, _maxSpawnRate, _startingSpawnFrequency);
    }

    void StartMatch()
    {
        StartCoroutine(HandleEnemySpawn());
    }

    IEnumerator HandleEnemySpawn()
    {
        while (GameManager.Instance.CanGameContinue())
        {
            SpawnEnemy();

            if (_nextSpawnInterval != -1)
            {
                _waitToSpawnEnemy = new WaitForSeconds(_nextSpawnInterval);
                _nextSpawnInterval = -1;
            }
            yield return _waitToSpawnEnemy;
        }
    }

    private void SpawnEnemy()
    {
        Vector2 randomCircleValue = Random.insideUnitCircle;
        Vector2 playerPos = MainPlayer.Instance.transform.position;

        Vector2 dirVector = (playerPos - (playerPos + randomCircleValue));
        dirVector.Normalize();

        Vector2 spawnPos = playerPos + (dirVector * Random.Range(_minDistanceSpawnedFromPlayer, _maxDistanceSpawnedFromPlayer));
        Enemy enemy = PoolableObjectsManager.Instance.GetPoolableObject(ObjectPoolTypes.Enemy) as Enemy;
        if (enemy)
        {
            enemy.transform.position = spawnPos;
            _enemiesInGame.Add(enemy);

            enemy.HealthComponent.OnKilled -= RemoveEnemyFromGame;
            enemy.HealthComponent.OnKilled += RemoveEnemyFromGame;
        }
    }

    public void RemoveEnemyFromGame(Character enemyKilled)
    {
        if (_enemiesInGame.Contains(enemyKilled as Enemy))
        {
            _enemiesInGame.Remove(enemyKilled as Enemy);
            enemyKilled.HealthComponent.OnKilled -= RemoveEnemyFromGame;
        }
    }
}
