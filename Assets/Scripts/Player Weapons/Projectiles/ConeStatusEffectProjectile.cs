using StatusEffects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Weaponry
{
    public class ConeStatusEffectProjectile : ConeProjectile
    {
        [SerializeField]
        private EStatusEffectType effectType;

        [SerializeField]
        private int _stacksToAdd = 1;

        protected override void OnContactWithEnemy(Enemy enemy)
        {
            base.OnContactWithEnemy(enemy);

            if (enemy)
            {
                StatusEffectsManager statusEffectsManager = enemy.StatusEffectsManager;
                if (statusEffectsManager)
                {
                    statusEffectsManager.IncrementStacks(effectType, _stacksToAdd);
                }
            }
        }
    }
}
