using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Weaponry
{
    public class FireAtNearestEnemyWeapon : PlayerWeapon
    {
        protected override void FireWeapon(ProjectileData data)
        {
            List<Enemy> enemies = EnemySpawnerController.Instance.EnemiesInGame;
            Enemy closestEnemy = GetNearestEnemy(enemies);
            if (closestEnemy)
            {
                Projectile projectile = Instantiate<Projectile>(_projectilePrefab, MainPlayer.Instance.transform.position, Quaternion.identity);

                projectile.InitializeProjectile(data, closestEnemy.transform.position - MainPlayer.Instance.transform.position);
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
