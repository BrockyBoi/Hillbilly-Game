using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Weaponry
{
    public class RevolveAroundPlayerWeapon : PlayerWeapon
    {
        List<RevolveAroundPlayerProjectile> _projectilesSpawned = new List<RevolveAroundPlayerProjectile>();
        protected override IEnumerator FireWeapon(ProjectileData projectileData)
        {
            for (int i = 0; i < WeaponData.NumberOfProjectilesToFire; i++)
            {
                float spawnRadians = i * (360 / WeaponData.NumberOfProjectilesToFire) * Mathf.Deg2Rad;
                Vector3 spawnPosition = MainPlayer.Instance.transform.position + (new Vector3(Mathf.Cos(spawnRadians), Mathf.Sin(spawnRadians)) * projectileData.ProjectileSizeMultiplier);

                RevolveAroundPlayerProjectile projectile = ProjectilePool.GetPoolableObject() as RevolveAroundPlayerProjectile;
                if (!(projectile))
                { 
                    Debug.LogError("Should be Revolve Around Player Weapon");
                    yield break;
                }
                projectile.InitializeProjectile(this, projectileData, i, WeaponData.NumberOfProjectilesToFire);
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
