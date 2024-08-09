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
            Vector3 playerMovementDir = MainPlayer.Instance.CharacterMovementComponent.LastMovementVector;
            Vector3 playerPos = MainPlayer.Instance.transform.position;
            playerMovementDir.Normalize();
            
            _boxCollider.transform.position = playerPos + new Vector3(Mathf.Cos(playerMovementDir.x), Mathf.Sin(playerMovementDir.y)) * _distanceInFrontOfPlayer;

            float angle = Vector3.Angle(Vector2.up, playerMovementDir);
            _boxCollider.transform.localRotation = Quaternion.Euler(0,0, angle);
        }
    }
}
