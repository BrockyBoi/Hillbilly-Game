using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Weaponry
{
    public class FireAtRandomEnemyWeapon : PlayerWeapon
    {
        protected override Vector3 GetDirectionToFireProjectile()
        {
            List<Enemy> enemies = EnemySpawnerController.Instance.EnemiesInGame;
            return GetRandomEnemy(enemies).transform.position - MainPlayer.Instance.transform.position;
        }

        protected Enemy GetRandomEnemy(List<Enemy> enemies)
        {
            return enemies[Random.Range(0, enemies.Count - 1)];
        }
    }
}
