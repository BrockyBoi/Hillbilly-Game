using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Weaponry
{
    public class RevolveAroundPlayerProjectile : Projectile
    {
        int _projectileIndex = 0;
        int _totalProjectiles = 0;
        float _totalTime = 0;

        protected override void MoveProjectile()
        {
            _totalTime += Time.deltaTime;
            float spawnRadians = _projectileIndex * (360 / _totalProjectiles) * Mathf.Deg2Rad;
            transform.position = MainPlayer.Instance.transform.position + (new Vector3(Mathf.Cos(spawnRadians + _totalTime), Mathf.Sin(spawnRadians + _totalTime)) * _projectileData.ProjectileSizeMultiplier);
        }

        public void InitializeProjectile(ProjectileData projectileData, int index, int totalProjectiles)
        {
            InitializeProjectile(projectileData);

            _projectileIndex = index;
            _totalProjectiles = totalProjectiles;
        }
    }
}
