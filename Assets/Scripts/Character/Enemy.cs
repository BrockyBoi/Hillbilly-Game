using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyAttackableComponent))]
[RequireComponent (typeof(EnemyMovementComponent))]
public class Enemy : Character
{
    private EnemyAttackableComponent _attackableComponent;

    protected override void Awake()
    {
        base.Awake();

        _attackableComponent = GetComponent<EnemyAttackableComponent>(); 
    }

    #region Collisions
    protected override void OnCollisionEnter2D(Collision2D other)
    {
        base.OnCollisionEnter2D(other);

        if (!IsAlive())
        {
            return;
        }

        MainPlayer mainPlayer = other.gameObject.GetComponent<MainPlayer>();
        if (mainPlayer != null)
        {
            _attackableComponent.AttackPlayer(mainPlayer);
        }
    }

    protected override void OnCollisionStay2D(Collision2D other)
    {
        base.OnCollisionStay2D(other);

        if (!IsAlive())
        {
            return;
        }

        MainPlayer mainPlayer = other.gameObject.GetComponent<MainPlayer>();
        if (mainPlayer != null)
        {
            _attackableComponent.AttackPlayer(mainPlayer);
        }
    }
    #endregion
}
