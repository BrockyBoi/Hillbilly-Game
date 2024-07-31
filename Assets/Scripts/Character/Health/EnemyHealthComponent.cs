using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XP;

public class EnemyHealthComponent : CharacterHealthComponent
{
    protected override void Die()
    {
        Enemy enemy = GetOwningCharacter() as Enemy;
        if (enemy)
        {
            EnemySpawnerController.Instance.RemoveEnemyFromGame(enemy);

            EnemySpawnerController.Instance.EnemyPool.AddObjectToPool(enemy);

            XPOrb xPOrb = XPPoolManager.Instance.XPOrbPool.GetPoolableObject(); ;
            if (xPOrb)
            {
                xPOrb.transform.position = transform.position;
                xPOrb.InitializeXPOrb(enemy.XPComponent.XpToGiveOnKill);
            }
        }

        base.Die();
    }
}
