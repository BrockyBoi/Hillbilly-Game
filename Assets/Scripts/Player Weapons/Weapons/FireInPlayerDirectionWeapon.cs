using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Weaponry
{
    public class FireInPlayerDirectionWeapon : PlayerWeapon
    {
        protected override Vector3 GetDirectionToFireProjectile()
        {
            Vector3 dirFacing = Vector3.zero;
            if (_mainPlayer)
            {
                dirFacing = _mainPlayer.CharacterMovementComponent.LastMovementVector;
            }

            return dirFacing;
        }
    }
}
