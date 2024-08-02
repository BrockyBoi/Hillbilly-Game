using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Weaponry
{
    public class BeamWeapon : FireAtRandomEnemyWeapon
    {
        List<BeamProjectile> _beamProjectiles = new List<BeamProjectile>();
        protected override IEnumerator FireWeapon(ProjectileData data)
        {
            for (int i = 0; i < WeaponData.NumberOfProjectilesToFire; i++)
            {
                BeamProjectile beamProjectile = _projectilePool.GetPoolableObject() as BeamProjectile;
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
