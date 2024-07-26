using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Weaponry
{
    public class FireInRandomDirectionWeapon : PlayerWeapon
    {
        protected override IEnumerator FireWeapon(ProjectileData data)
        {
            Vector3 randomLocation = Random.insideUnitCircle;
            for (int i = 0; i < WeaponData.NumberOfProjectilesToFire; i++)
            {
                Projectile projectile = Instantiate<Projectile>(WeaponData.ProjectilePrefab, MainPlayer.Instance.transform.position, Quaternion.identity);

                Vector3 dir = (randomLocation + Random.insideUnitSphere) - MainPlayer.Instance.transform.position;
                dir.Normalize();

                projectile.InitializeProjectile(data, dir);

                yield return _waitForInBetweenProjectiles;
            }
        } 
    }
}
