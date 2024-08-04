using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class EnemySpawnerController : MonoBehaviour
{
    public static EnemySpawnerController Instance { get; private set; }

    private float _currentSpawnFrequency;

    [Title("Spawn Parameters")]
    [SerializeField]
    private float _maxSpawnRate = .5f;
    public float MaxSpawnRate { get { return _maxSpawnRate; } }

    [SerializeField]
    private float _startingSpawnFrequency = 2.5f;
    public float StartingSpawnFrequency { get { return _startingSpawnFrequency; } }

    private float _nextSpawnInterval = -1;

    [Title("Spawn Location")]
    [SerializeField]
    private float _minDistanceSpawnedFromPlayer = 25;

    [SerializeField]
    private float _maxDistanceSpawnedFromPlayer = 50;

    WaitForSeconds _waitToSpawnEnemy;

    private List<Enemy> _enemiesInGame = new List<Enemy>();
    public List<Enemy> EnemiesInGame { get { return _enemiesInGame; } }

    [SerializeField]
    private GameObject _enemyPrefab;

    private PoolableObjectsComponent<Enemy> _enemyPool = new PoolableObjectsComponent<Enemy>();
    public PoolableObjectsComponent<Enemy> EnemyPool { get { return _enemyPool; } }

    EnemyDifficultyManager _enemyDifficultyManager;

    private void Awake()
    {
        Instance = this;
        _enemyDifficultyManager = GetComponent<EnemyDifficultyManager>();
    }

    private void OnEnable()
    {
        _enemyDifficultyManager.OnDifficultyChange -= OnSpawnIntervalChange;
        _enemyDifficultyManager.OnDifficultyChange += OnSpawnIntervalChange;
    }

    private void OnDisable()
    {
        _enemyDifficultyManager.OnDifficultyChange -= OnSpawnIntervalChange;
    }

    // Start is called before the first frame update
    void Start()
    {
        _currentSpawnFrequency = _startingSpawnFrequency;

        _waitToSpawnEnemy = new WaitForSeconds(_currentSpawnFrequency);
        _enemyPool.Initialize(_enemyPrefab);

        StartMatch();
    }

    // Update is called once per frame
    void Update()
    {


    }

    private void OnSpawnIntervalChange()
    {
        _nextSpawnInterval = _enemyDifficultyManager.CurrentSpawnFrequency;
        _currentSpawnFrequency = _nextSpawnInterval;
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
        Enemy enemy = _enemyPool.GetPoolableObject();
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

    public Enemy GetRandomEnemy()
    {
        return _enemiesInGame.Count > 0 ? _enemiesInGame[Random.Range(0, _enemiesInGame.Count - 1)] : null;
    }

    public List<Enemy> GetRandomEnemies(int desiredEnemyCount)
    {
        List<Enemy> enemies = new List<Enemy>(_enemiesInGame);
        for (int i = 0; i < desiredEnemyCount; i++)
        {
            Enemy randomEnemy = null;
            while ((!randomEnemy || enemies.Contains(randomEnemy)) && enemies.Count > 0)
            {
                int randomIndex = Random.Range(0, enemies.Count - 1);
                randomEnemy = enemies[randomIndex];
                enemies.RemoveAt(randomIndex);
            }

            if (randomEnemy)
            {
                enemies.Add(randomEnemy);
            }
        }

        return enemies;
    }

    public List<Enemy> GetRandomEnemiesWithinRange(int desiredEnemyCount, Vector3 startPos, float distance)
    {
        List<Enemy> enemies = new List<Enemy>(_enemiesInGame.Where(enemy => Vector3.Distance(startPos, enemy.transform.position) <= distance).ToList());
        List<Enemy> enemiesInRange = new List<Enemy>();
        for (int i = 0; i < desiredEnemyCount; i++)
        {
            Enemy randomEnemy = null;
            while ((!randomEnemy || enemies.Contains(randomEnemy)) && enemies.Count > 0)
            {
                int randomIndex = Random.Range(0, enemies.Count - 1);
                randomEnemy = enemies[randomIndex];
                enemies.RemoveAt(randomIndex);
            }

            if (randomEnemy)
            {
                enemiesInRange.Add(randomEnemy);
            }
        }

        return enemiesInRange;
    }
}
