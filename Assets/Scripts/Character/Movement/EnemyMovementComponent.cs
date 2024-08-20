using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovementComponent : CharacterMovementComponent
{
    MainPlayer _player;
    EnemyAttackableComponent _attackableComponent;
    protected override void Awake()
    {
        base.Awake();

        _attackableComponent = GetComponent<EnemyAttackableComponent>();
    }

    protected override void Start()
    {
        base.Start();

        _player = MainPlayer.Instance;
    }

    protected override void Move()
    {
        if (_attackableComponent && _attackableComponent.IsAttacking)
        {
            return;
        }

        Vector3 playerPosition = _player.transform.position;
        Vector3 thisPosition = transform.position;

        Vector3 directionVector = (playerPosition - thisPosition);
        directionVector.Normalize();

        Vector3 movementVector = (directionVector * Time.deltaTime * _movementSpeed * _movementSpeedModifier);
        transform.position = thisPosition + movementVector;

        _lastMovementVector = movementVector;
    }
}
