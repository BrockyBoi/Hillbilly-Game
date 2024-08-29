using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Weaponry
{
    public class FireAtRandomPointNearPlayerWeapon : PlayerWeapon
    {
        [Title("Random Point Near Play Weapon Stats")]
        [SerializeField]
        protected float _maxDistanceFromPlayer = 10f;

        protected override Vector3 GetDirectionToFireProjectile()
        {
            return transform.position + (Random.insideUnitSphere * _maxDistanceFromPlayer);
        }
    }
}
