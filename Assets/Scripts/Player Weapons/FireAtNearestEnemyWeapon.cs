using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Weaponry
{
    public class FireAtNearestEnemyWeapon : PlayerWeapon
    {
        protected override IEnumerator FireWeapon(ProjectileData data)
        {
            List<Enemy> enemies = EnemySpawnerController.Instance.EnemiesInGame;
            Enemy closestEnemy = GetNearestEnemy(enemies);
            if (closestEnemy)
            {
                for (int i = 0; i < WeaponData.NumberOfProjectilesToFire; i++)
                {
                    Projectile projectile = Instantiate<Projectile>(WeaponData.ProjectilePrefab, MainPlayer.Instance.transform.position, Quaternion.identity);
                    projectile.InitializeProjectile(this, data, closestEnemy.transform.position - MainPlayer.Instance.transform.position);
                }

                yield return _waitForInBetweenProjectiles;
            }
        }

        private Enemy GetNearestEnemy(List<Enemy> enemies)
        {
            float minDistance = float.MaxValue;
            Enemy closestEnemy = null;

            MainPlayer player = MainPlayer.Instance;
            Vector3 playerPos = player.transform.position;
            foreach (Enemy enemy in enemies)
            {
                if (enemy)
                {
                    Vector3 offset = playerPos - enemy.transform.position;
                    if (offset.sqrMagnitude < minDistance)
                    {
                        minDistance = offset.sqrMagnitude;
                        closestEnemy = enemy;
                    }
                }
            }

            return closestEnemy;
        }
    }
}
