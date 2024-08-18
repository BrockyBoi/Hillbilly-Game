using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDifficultyManager : MonoBehaviour
{
    public static EnemyDifficultyManager Instance { get; private set; }
    private EnemySpawnerController _enemySpawnerController;

    public delegate void EOnDifficultyChange();
    public event EOnDifficultyChange OnDifficultyChange;


    [Title("Difficulty")]
    [SerializeField]
    private float _timeBetweenDifficultyChange = 15f;

    [Title("Spawn Frequency")]
    [SerializeField]
    private float _timeToTakeOffOfSpawnFrequency = .3f;
    private float _currentSpawnFrequency;
    public float CurrentSpawnFrequency { get { return _currentSpawnFrequency; } }

    [Title("Health")]
    [SerializeField]
    private float _amountToIncreaseHealthMultiplier = .25f;
    public float CurrentHealthMultiplier { get { return 1f + (_currentDifficultyLevel * _amountToIncreaseHealthMultiplier); } }

    [Title("XP")]
    [SerializeField]
    private int _xpAmountToIncreasePerLevel = 15;
    public float CurrentXPBonusModifier { get { return 1f + (_currentDifficultyLevel * _xpAmountToIncreasePerLevel); } }

    [Title("Speed")]
    [SerializeField]
    private float _speedModifierIncreaseOnDifficultyChange = .25f;
    public float CurrentEnemyMovementSpeedModifier { get { return 1f + (_currentDifficultyLevel * _speedModifierIncreaseOnDifficultyChange); } }


    private int _currentDifficultyLevel = 0;

    private WaitForSeconds _waitToIncreaseDifficulty;

    private void Awake()
    {
        Instance = this;
        _enemySpawnerController = GetComponent<EnemySpawnerController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        _waitToIncreaseDifficulty = new WaitForSeconds(_timeBetweenDifficultyChange);
        _currentSpawnFrequency = EnemySpawnerController.Instance.StartingSpawnFrequency;
        StartCoroutine(HandleIncreaseDifficulty());
    }

    IEnumerator HandleIncreaseDifficulty()
    {
        while (GameManager.Instance.CanGameContinue())
        {
            yield return _waitToIncreaseDifficulty;

            IncreaseDifficulty();
        }
    }

    private void IncreaseDifficulty()
    {
        if (_enemySpawnerController)
        {
            _currentDifficultyLevel++;
            _currentSpawnFrequency = Mathf.Clamp(_currentSpawnFrequency - _timeToTakeOffOfSpawnFrequency, _enemySpawnerController.MaxSpawnRate, _enemySpawnerController.StartingSpawnFrequency);

            Debug.Log("New difficulty level " + _currentDifficultyLevel);
            OnDifficultyChange?.Invoke();
        }
    }

    public float GetBonusXP()
    {
        return MainPlayer.Instance.UpgradeAttributesComponent.GetModifiedAttributeValue(UpgradeAttribute.XPMultiplier, CurrentXPBonusModifier);
    }
}
