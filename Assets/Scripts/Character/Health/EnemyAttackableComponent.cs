using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackableComponent : MonoBehaviour
{
    [SerializeField]
    private float _damageToDeal = 15f;

    [SerializeField]
    private float _timeBetweenAttacks = .75f;

    WaitForSeconds _waitBetweenAttacks;

    private bool _isInAttackCooldown = false;

    private Enemy _owningEnemy;

    private void Start()
    {
        _owningEnemy = GetComponent<Enemy>();
        _waitBetweenAttacks = new WaitForSeconds(_timeBetweenAttacks);
    }

    public void AttackPlayer(MainPlayer playerToAttack)
    {
        if (CanAttack())
        {
            playerToAttack.HealthComponent?.DoDamage(_damageToDeal);

            StartCoroutine(WaitUntilAttackCooldownOver());
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

    private bool CanAttack()
    {
        return !_isInAttackCooldown && !_owningEnemy.IsFrozen();
    }
}
