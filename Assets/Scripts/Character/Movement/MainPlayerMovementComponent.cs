using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainPlayerMovementComponent : CharacterMovementComponent
{
    protected override void Move()
    {
        Vector3 directionVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        directionVector.Normalize();
        MainPlayer player = _owningCharacter as MainPlayer;
        float movementSpeedModifier = player.UpgradeAttributesComponent.GetAttribute(UpgradeAttribute.MovementSpeed);
        Vector3 moveVector = directionVector * Time.deltaTime * _movementSpeed * movementSpeedModifier;
        transform.position = transform.position + moveVector;

        _lastMovementVector = moveVector;
    }
}
