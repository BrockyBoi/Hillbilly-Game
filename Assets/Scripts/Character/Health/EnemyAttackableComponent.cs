using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackableComponent : MonoBehaviour
{
    [SerializeField]
    private float _damageToDeal = 15f;

    [SerializeField]
    private float _timeBetweenAttacks = .75f;

    [SerializeField]
    private float _timeUntilAttack = .5f;

    WaitForSeconds _waitBetweenAttacks;

    private bool _isAttacking = false;
    public bool IsAttacking {  get { return _isAttacking; } }

    private bool _isInAttackCooldown = false;

    private Enemy _owningEnemy;

    public delegate void EOnAttackStart();
    public event EOnAttackStart OnAttackStart;

    private void Awake()
    {
        _owningEnemy = GetComponent<Enemy>();
        _waitBetweenAttacks = new WaitForSeconds(_timeBetweenAttacks);
    }

    private void OnEnable()
    {
        _owningEnemy.HealthComponent.OnKilled -= OnEnemyKilled;
        _owningEnemy.HealthComponent.OnKilled += OnEnemyKilled;
    }

    private void OnDisable()
    {
        _owningEnemy.HealthComponent.OnKilled -= OnEnemyKilled;
    }

    private void OnEnemyKilled(Character enemyKilled)
    {
        StopAllCoroutines();
        _isAttacking = false;
    }

    public void AttackPlayer(MainPlayer playerToAttack)
    {
        if (CanAttack())
        {
            StartCoroutine(StartAttack(playerToAttack));
        }
    }

    public float GetDamageToDeal()
    {
        return _damageToDeal;
    }

    private IEnumerator WaitUntilAttackCooldownOver()
    {
        _isInAttackCooldown = true;

        yield return _waitBetweenAttacks;

        _isInAttackCooldown = false;
    }

    private IEnumerator StartAttack(MainPlayer playerToAttack)
    {
        _isAttacking = true;
        yield return new WaitForSeconds(_timeUntilAttack);

        OnAttackStart?.Invoke();

        if (_owningEnemy && _owningEnemy.IsTouchingPlayer && playerToAttack)
        {
            playerToAttack.HealthComponent?.DoDamage(_damageToDeal);
        }

        _isAttacking = false;
        yield return WaitUntilAttackCooldownOver();
    }

    private bool CanAttack()
    {
        return !_isInAttackCooldown && !_owningEnemy.IsFrozen() && !_isAttacking;
    }
}
