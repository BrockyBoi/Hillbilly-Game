using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnerController : MonoBehaviour
{
    public static EnemySpawnerController Instance { get; private set; }

    private float _currentSpawnFrequency;

    [Title("Prefabs")]
    [Required, SerializeField]
    private Enemy _enemyToSpawn;

    [Title("Spawn Parameters")]
    [SerializeField]
    private float _maxSpawnRate = .5f;

    [SerializeField]
    private float _startingSpawnFrequency = 2.5f;

    [Title("Difficulty")]
    [SerializeField]
    private float _timeBetweenDifficultyChange = 15f;

    [SerializeField]
    private float _timeToTakeOffOfSpawnFrequency = .3f;

    [Title("Spawn Location")]
    [SerializeField]
    private float _minDistanceSpawnedFromPlayer = 25;

    [SerializeField]
    private float _maxDistanceSpawnedFromPlayer = 50;

    WaitForSeconds _waitToSpawnEnemy;
    WaitForSeconds _waitToChangeDifficulty;

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
        _waitToChangeDifficulty = new WaitForSeconds(_timeBetweenDifficultyChange);


        StartMatch();
    }

    // Update is called once per frame
    void Update()
    {


    }

    void StartMatch()
    {
        StartCoroutine(HandleEnemySpawn());
        StartCoroutine(HandleSpawnRateIncrease());
    }

    IEnumerator HandleEnemySpawn()
    {
        while (GameManager.Instance.CanGameContinue())
        {
            SpawnEnemy();

            yield return _waitToSpawnEnemy;
        }
    }

    IEnumerator HandleSpawnRateIncrease()
    {
        while (GameManager.Instance.CanGameContinue())
        {
            yield return _waitToChangeDifficulty;

            _currentSpawnFrequency = Mathf.Clamp(_currentSpawnFrequency - _timeToTakeOffOfSpawnFrequency, _maxSpawnRate, _startingSpawnFrequency);
            _waitToSpawnEnemy = new WaitForSeconds(_currentSpawnFrequency);
        }
    }


    private void SpawnEnemy()
    {
        Vector2 randomCircleValue = Random.insideUnitCircle;
        Vector2 playerPos = MainPlayer.Instance.transform.position;

        Vector2 dirVector = (playerPos - (playerPos + randomCircleValue));
        dirVector.Normalize();

        Vector2 spawnPos = playerPos + (dirVector * Random.Range(_minDistanceSpawnedFromPlayer, _maxDistanceSpawnedFromPlayer));
        Enemy enemy = Instantiate<Enemy>(_enemyToSpawn, spawnPos, Quaternion.identity);
        _enemiesInGame.Add(enemy);

        enemy.HealthComponent.OnKilled -= RemoveEnemyFromGame;
        enemy.HealthComponent.OnKilled += RemoveEnemyFromGame;
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
