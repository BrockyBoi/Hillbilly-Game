using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnerController : MonoBehaviour
{
    private float _currentSpawnFrequency;

    [Title("Prefabs")]
    [Required, SerializeField]
    private GameObject _enemyToSpawn;

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


    void SpawnEnemy()
    {
        Debug.Log("Spawn enemy");
        Vector2 randomCircleValue = Random.insideUnitCircle;
        Vector2 playerPos = MainPlayer.Instance.transform.position;

        Vector2 dirVector = (playerPos - (playerPos + randomCircleValue));
        dirVector.Normalize();

        Vector2 spawnPos = playerPos + (dirVector * Random.Range(_minDistanceSpawnedFromPlayer, _maxDistanceSpawnedFromPlayer));
        Instantiate(_enemyToSpawn, spawnPos, Quaternion.identity);
    }
}
