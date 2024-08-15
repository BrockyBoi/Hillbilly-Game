using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Weaponry
{
    public class ShotgunEffectWeapon : FireInPlayerDirectionWeapon
    {
        [SerializeField]
        private float _coneAngle = 45;

        protected override IEnumerator FireWeapon(ProjectileData data)
        {
            int numberOfProjectiles = Mathf.RoundToInt(WeaponData.NumberOfProjectilesToFire + WeaponAttributesComponent.GetAttribute(UpgradeAttribute.NumberOfProjectiles));
            Vector3 baseDirVector = GetDirectionToFireProjectile();
            float angleSlice = _coneAngle / numberOfProjectiles;
            float halfAngle = _coneAngle / 2;
            float startingAngle = Vector3.Angle(_mainPlayer.transform.up, baseDirVector);

            for (int i = 1; i <= numberOfProjectiles; i++)
            {
                BaseProjectile projectile = _projectilePool.GetPoolableObject();
                if (projectile)
                {
                    projectile.transform.position = _mainPlayer.transform.position;
                    float projectileRadians = startingAngle +  (-halfAngle + (angleSlice * i)) * Mathf.Deg2Rad;
                    Vector3 projectileVector = baseDirVector + new Vector3(Mathf.Cos(projectileRadians), Mathf.Sin(projectileRadians));

                    projectile.InitializeProjectile(this, data, projectileVector);
                }
                else
                {
                    Debug.LogError("Projectile is null");
                }
            }

            yield return _waitForInBetweenProjectiles;
        }
    }
}
