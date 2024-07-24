using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainPlayerMovementComponent : CharacterMovementComponent
{
    protected override void Move()
    {
        Vector3 directionVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        Vector3 moveVector = directionVector * Time.deltaTime * _movementSpeed;

        transform.position = transform.position + moveVector;
    }
}
