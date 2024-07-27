using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthComponent : CharacterHealthComponent
{
    protected override void Die()
    {
        Enemy enemy = GetOwningCharacter() as Enemy;
        if (enemy)
        {
            EnemySpawnerController.Instance.RemoveEnemyFromGame(enemy);

            PoolableObjectsManager.Instance.AddObjectToPool(enemy, ObjectPoolTypes.Enemy);

            XPOrb xPOrb = PoolableObjectsManager.Instance.GetPoolableObject(ObjectPoolTypes.XPOrb) as XPOrb;
            if (xPOrb)
            {
                xPOrb.transform.position = transform.position;
                xPOrb.InitializeXPOrb(enemy.XPComponent.XpToGiveOnKill);
            }
        }

        base.Die();
    }
}
