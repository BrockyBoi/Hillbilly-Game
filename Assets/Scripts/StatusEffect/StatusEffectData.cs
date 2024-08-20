using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StatusEffects
{
    public enum EStatusEffectType
    {
        None,
        Slow,
        Freeze,
        Electrocuted,
        Burn,
        Dazed,
        Vulnerable,
        Poison,
        Weak
    }

    [System.Serializable]
    public struct StatusEffectData
    {
        [Title("Base Data")]
        public EStatusEffectType StatusEffectType;

        public bool IsPermanentStatusEffect;

        [Range(1, 999)]
        public int MaxStacks;

        [Range(.1f, 10)]
        public float TimeToLoseStack;

        [Title("Damage")]
        public bool CanApplyDamage;

        [ShowIf("CanApplyDamage")]
        public bool ApplyDamageOnLoseStack;

        [ShowIf("CanApplyDamage")]
        public float DamagePerStack;

        [ShowIf("CanApplyDamage")]
        public float HealthLossPercentagePerStack;

        [Title("Extra Status Effect")]
        public bool CanApplyNewStatusEffect;

        [ShowIf("CanApplyNewStatusEffect")]
        public int StacksNeededForNewEffect;

        [ShowIf("CanApplyNewStatusEffect")]
        public EStatusEffectType NewStatusEffectType;
    }

    [Serializable]
    public class StatusEffectAttackData
    {
        public EStatusEffectType EffectType;
        public int StacksToAdd = 1;
    }
}
