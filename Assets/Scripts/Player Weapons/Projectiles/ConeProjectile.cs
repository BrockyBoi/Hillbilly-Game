using StatusEffects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Weaponry
{
    public class ConeProjectile : BaseProjectile
    {
        [SerializeField]
        protected float _distanceInFrontOfPlayer = 15;

        protected override void MoveProjectile()
        {
            MainPlayer player = MainPlayer.Instance;
            Vector3 playerMovementDir = player.CharacterMovementComponent.LastMovementVector == Vector2.zero ? player.transform.up : player.CharacterMovementComponent.LastMovementVector;
            playerMovementDir.Normalize();

            Vector3 playerPos = player.transform.position;
            
            _boxCollider.transform.position = playerPos + (playerMovementDir * _distanceInFrontOfPlayer);

            float angle = Vector3.Angle(player.transform.up, playerMovementDir);
            if (playerMovementDir.x > 0)
            {
                angle *= -1;
            }

            _boxCollider.transform.localRotation = Quaternion.Euler(0,0, angle);
        }
    }
}
