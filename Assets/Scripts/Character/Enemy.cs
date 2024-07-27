using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XP;

[RequireComponent(typeof(EnemyAttackableComponent))]
[RequireComponent(typeof(EnemyMovementComponent))]
[RequireComponent(typeof(EnemyHealthComponent))]
[RequireComponent(typeof(EnemyXPComponent))]
public class Enemy : Character
{
    private EnemyAttackableComponent _attackableComponent;

    protected override void Awake()
    {
        base.Awake();

        _attackableComponent = GetComponent<EnemyAttackableComponent>(); 
    }

    #region Collisions
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);

        if (!IsAlive())
        {
            return;
        }

        MainPlayer mainPlayer = other.gameObject.GetComponent<MainPlayer>();
        if (mainPlayer != null)
        {
            _attackableComponent?.AttackPlayer(mainPlayer);
        }
    }

    protected override void OnTriggerStay2D(Collider2D other)
    {
        base.OnTriggerStay2D(other);

        if (!IsAlive())
        {
            return;
        }

        MainPlayer mainPlayer = other.gameObject.GetComponent<MainPlayer>();
        if (mainPlayer != null)
        {
            _attackableComponent?.AttackPlayer(mainPlayer);
        }
    }
    #endregion
}
