using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XP;

[RequireComponent(typeof(EnemyAttackableComponent))]
[RequireComponent(typeof(EnemyMovementComponent))]
[RequireComponent(typeof(EnemyHealthComponent))]
[RequireComponent(typeof(EnemyXPComponent))]
public class Enemy : Character, IPoolableObject 
{
    private EnemyAttackableComponent _attackableComponent;
    private EnemyXPComponent _xpComponent;
    public EnemyXPComponent XPComponent { get { return _xpComponent; } }

    protected override void Awake()
    {
        base.Awake();

        _attackableComponent = GetComponent<EnemyAttackableComponent>();
        _xpComponent = GetComponent<EnemyXPComponent>();
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

    public void ActivateObject(bool shouldActivate)
    {
        gameObject.SetActive(shouldActivate);
        _spriteRenderer.enabled = shouldActivate;
        _boxCollider.enabled = shouldActivate;
    }
}
