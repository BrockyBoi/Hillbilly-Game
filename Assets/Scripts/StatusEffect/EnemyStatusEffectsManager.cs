using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StatusEffects
{
    public class EnemyStatusEffectsManager : StatusEffectsManager
    {
        protected override void Start()
        {
            base.Start();

            if (_owningCharacter)
            {
                EnemyAttackableComponent enemyAttackableComponent = GetComponent<EnemyAttackableComponent>();
                if (enemyAttackableComponent)
                {
                    enemyAttackableComponent.OnAttackStart -= OnEnemyAttack;
                    enemyAttackableComponent.OnAttackStart += OnEnemyAttack;
                }
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (_owningCharacter)
            {
                EnemyAttackableComponent enemyAttackableComponent = GetComponent<EnemyAttackableComponent>();
                if (enemyAttackableComponent)
                {
                    enemyAttackableComponent.OnAttackStart -= OnEnemyAttack;

                }
            }
        }


        void OnEnemyAttack()
        {
            int electrocutionStacks = GetStacks(EStatusEffectType.Electrocuted);
            if (electrocutionStacks > 0)
            {
                AttemptDamageCharacterWithStatusEffect(EStatusEffectType.Electrocuted);
            }
        }
    }
}
