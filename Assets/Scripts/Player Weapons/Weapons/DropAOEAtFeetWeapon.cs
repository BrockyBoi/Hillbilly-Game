using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Weaponry
{
    public class DropAOEAtFeetWeapon : PlayerWeapon
    {
        protected override Vector3 GetDirectionToFireProjectile()
        {
            return Vector3.zero;
        }
    }
}
