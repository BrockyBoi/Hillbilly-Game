using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Weaponry
{
    public class RevolveAroundPlayerWeapon : PlayerWeapon
    {
        List<Projectile> _projectilesSpawned = new List<Projectile>();
        protected override IEnumerator FireWeapon(ProjectileData projectileData)
        {
            for (int i = 0; i < WeaponData.NumberOfProjectilesToFire; i++)
            {
                float spawnRadians = i * (360 / WeaponData.NumberOfProjectilesToFire) * Mathf.Deg2Rad;
                Vector3 spawnPosition = MainPlayer.Instance.transform.position + (new Vector3(Mathf.Cos(spawnRadians), Mathf.Sin(spawnRadians)) * projectileData.ProjectileSizeMultiplier);

                RevolveAroundPlayerProjectile projectile = Instantiate<Projectile>(WeaponData.ProjectilePrefab, spawnPosition, Quaternion.identity) as RevolveAroundPlayerProjectile;
                if (!(projectile))
                { 
                    Debug.LogError("Should be Revolve Around Player Weapon");
                    yield break;
                }
                projectile.InitializeProjectile(projectileData, i, WeaponData.NumberOfProjectilesToFire);
                _projectilesSpawned.Add(projectile);
            }

            yield return new WaitForSeconds(projectileData.WeaponDuration);

            for (int i = _projectilesSpawned.Count - 1; i >= 0; i--)
            {
                Destroy(_projectilesSpawned[i].gameObject);
            }

            yield return new WaitForSeconds(3);
        }
    }
}
