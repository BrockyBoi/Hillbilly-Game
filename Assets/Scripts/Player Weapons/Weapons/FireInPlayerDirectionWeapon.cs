using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Weaponry
{
    public class FireInPlayerDirectionWeapon : PlayerWeapon
    {
        MainPlayer _player;

        protected override void OnEnable()
        {
            base.OnEnable();

            _player = MainPlayer.Instance;
        }

        protected override Vector3 GetDirectionToFireProjectile()
        {
            Vector3 dirFacing = Vector3.zero;
            if (_player)
            {
                dirFacing = _player.CharacterMovementComponent.LastMovementVector;
            }

            return dirFacing;
        }
    }
}
