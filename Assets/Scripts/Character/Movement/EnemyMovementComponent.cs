using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovementComponent : CharacterMovementComponent
{
    protected override void Move()
    {
        MainPlayer player = MainPlayer.Instance;
        Vector3 playerPosition = player.transform.position;
        Vector3 thisPosition = transform.position;

        Vector3 directionVector = (playerPosition - thisPosition);
        directionVector.Normalize();

        Vector3 movementVector = (directionVector * Time.deltaTime * _movementSpeed * _movementSpeedModifier);
        transform.position = thisPosition + movementVector;

        _lastMovementVector = movementVector;
    }
}
