using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovementComponent : CharacterMovementComponent
{
    protected override void Move()
    {
        MainPlayer player = MainPlayer.Instance;
        Vector2 playerPosition = player.transform.position;
        Vector2 thisPosition = transform.position;

        Vector2 directionVector = (playerPosition - thisPosition);
        directionVector.Normalize();
        Vector2 movementVector = directionVector * Time.deltaTime * _movementSpeed;
        transform.position = thisPosition + movementVector;
    }
}
