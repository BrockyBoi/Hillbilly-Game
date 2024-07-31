using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Weaponry
{
    public class BeamWeapon : FireAtRandomEnemyWeapon
    {
        protected override IEnumerator FireWeapon(ProjectileData data)
        {
            Enemy randomEnemy = GetRandomEnemy(EnemySpawnerController.Instance.EnemiesInGame);
            yield return null;
        }
    }
}
