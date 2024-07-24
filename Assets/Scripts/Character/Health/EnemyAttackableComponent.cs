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

    private bool _canAttack = true;

    private void Start()
    {
        _waitBetweenAttacks = new WaitForSeconds(_timeBetweenAttacks);
    }

    public void AttackPlayer(MainPlayer playerToAttack)
    {
        if (_canAttack)
        {
            Debug.Log("Attack Player");
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
        _canAttack = false;

        yield return _waitBetweenAttacks;

        _canAttack = true;
    }
}
