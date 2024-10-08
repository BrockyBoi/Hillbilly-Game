using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Weaponry
{
    public class ShotgunEffectWeapon : FireInPlayerDirectionWeapon
    {
        [SerializeField]
        private float _coneDegrees = 45;

        protected override IEnumerator FireWeapon(ProjectileData data)
        {
            int numberOfProjectiles = Mathf.RoundToInt(WeaponAttributesComponent.GetModifiedAttributeValue(UpgradeAttribute.NumberOfProjectiles, WeaponData.NumberOfProjectilesToFire));
            Vector3 baseDirVector = GetDirectionToFireProjectile();
            float angleSlice = _coneDegrees / numberOfProjectiles;
            float halfAngle = _coneDegrees / 2;
            float startingDegrees = Vector3.Angle(_owningPlayer.transform.up, baseDirVector);

            for (int i = 1; i <= numberOfProjectiles; i++)
            {
                BaseProjectile projectile = _projectilePool.GetPoolableObject();
                if (projectile)
                {
                    projectile.transform.position = _owningPlayer.transform.position;
                    float projectileRadians = (startingDegrees +  (-halfAngle + (angleSlice * i))) * Mathf.Deg2Rad;
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
