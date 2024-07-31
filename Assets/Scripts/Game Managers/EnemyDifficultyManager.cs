using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDifficultyManager : MonoBehaviour
{
    public static EnemyDifficultyManager Instance { get; private set; }
    [Title("Difficulty")]
    [SerializeField]
    private float _timeBetweenDifficultyChange = 15f;

    [Title("Spawn Frequency")]
    [SerializeField]
    private float _timeToTakeOffOfSpawnFrequency = .3f;

    [SerializeField]
    private float _startingSpawnFrequency = 2.5f;

    [SerializeField]
    private float _maxSpawnRate = .1f;
    private float _currentSpawnFrequency;

    [Title("Health")]
    [SerializeField]
    private float _amountToIncreaseHealthMultiplier = .25f;
    private float _currentHealthMultiplier;
    public float CurrentHealthMultiplier { get { return _currentHealthMultiplier; } }

    [Title("XP")]
    [SerializeField]
    private int _xpAmountToIncreasePerLevel = 15;
    private int _currentXPBonusAmount = 0;
    public int CurrentXPBonusAmount { get { return _currentXPBonusAmount; } }


    private int _currentDifficultyLevel = 0;

    private WaitForSeconds _waitToIncreaseDifficulty;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        _waitToIncreaseDifficulty = new WaitForSeconds(_timeBetweenDifficultyChange);
        StartCoroutine(HandleIncreaseDifficulty());
    }

    // Update is called once per frame
    void Update()
    {
        
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
        _currentDifficultyLevel++;
        _currentSpawnFrequency = Mathf.Clamp(_currentSpawnFrequency - _timeToTakeOffOfSpawnFrequency, _maxSpawnRate, _startingSpawnFrequency);
        _currentHealthMultiplier += _amountToIncreaseHealthMultiplier;

        EnemySpawnerController.Instance.SetSpawnInterval(_currentSpawnFrequency);
        Debug.Log("New difficulty level " + _currentDifficultyLevel);
    }

    public int GetBonusXP()
    {
        return Mathf.RoundToInt(_currentDifficultyLevel * _xpAmountToIncreasePerLevel * MainPlayer.Instance.UpgradeAttributesComponent.GetAttribute(UpgradeAttribute.XPMultiplier));
    }
}
