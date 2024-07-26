using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthComponent : CharacterHealthComponent
{
    protected override void Die()
    {
        EnemySpawnerController.Instance.RemoveEnemyFromGame(GetOwningCharacter() as Enemy);

        Destroy(gameObject);

        base.Die();
    }
}
