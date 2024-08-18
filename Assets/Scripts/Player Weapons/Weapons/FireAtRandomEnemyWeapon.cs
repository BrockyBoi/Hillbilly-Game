using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Weaponry
{
    public class FireAtRandomEnemyWeapon : PlayerWeapon
    {
        protected override Vector3 GetDirectionToFireProjectile()
        {
            Enemy randomEnmy = GetRandomEnemy();
            if (!randomEnmy)
            {
                return Vector3.zero;
            }

            return GetRandomEnemy().transform.position - MainPlayer.Instance.transform.position;
        }

        public Enemy GetRandomEnemy()
        {
            List<Enemy> enemies = EnemySpawnerController.Instance.EnemiesInGame;
            return enemies.Count > 0 ? enemies[Random.Range(0, enemies.Count - 1)] : null;
        }
    }
}
