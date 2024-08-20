using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Weaponry
{
    public class BeamWeapon : FireAtRandomEnemyWeapon
    {
        List<BaseBeamProjectile> _beamProjectiles = new List<BaseBeamProjectile>();
        protected override IEnumerator FireWeapon(ProjectileData data)
        {
            for (int i = 0; i < WeaponData.NumberOfProjectilesToFire; i++)
            {
                BaseBeamProjectile beamProjectile = _projectilePool.GetPoolableObject() as BaseBeamProjectile;
                if (beamProjectile)
                {
                    beamProjectile.InitializeProjectile(this, data);
                    _beamProjectiles.Add(beamProjectile);
                }
            }

            yield return new WaitForSeconds(data.WeaponDuration + WeaponData.TimeBetweenProjectiles);

            for (int i = 0; i < _beamProjectiles.Count; i++)
            {
                _projectilePool.AddObjectToPool(_beamProjectiles[i]);
            }

            _beamProjectiles.Clear();
        }
    }
}
