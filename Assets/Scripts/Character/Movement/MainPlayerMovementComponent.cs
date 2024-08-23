using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainPlayerMovementComponent : CharacterMovementComponent
{
    protected override void Move()
    {
        Vector3 directionVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if (directionVector != Vector3.zero)
        {
            directionVector.Normalize();
            MainPlayer player = _owningCharacter as MainPlayer;

            float movementSpeedModifier = player.UpgradeAttributesComponent.GetModifiedAttributeValue(UpgradeAttribute.MovementSpeed, _movementSpeed);
            Vector3 moveVector = directionVector * Time.deltaTime * movementSpeedModifier;
            transform.position = transform.position + moveVector;

            moveVector.Normalize();
            _lastMovementVector = moveVector;
        }
    }
}
