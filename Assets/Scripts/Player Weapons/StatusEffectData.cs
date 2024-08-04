using Sirenix.OdinInspector;
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
        public bool CanApplyDamageOnStackLoss;

        [ShowIf("CanApplyDamageOverTime")]
        public float DamagePerStack;

        [Title("Extra Status Effect")]
        public bool CanApplyNewStatusEffect;

        [ShowIf("CanApplyNewStatusEffect")]
        public int StacksNeededForNewEffect;

        [ShowIf("CanApplyNewStatusEffect")]
        public EStatusEffectType NewStatusEffectType;
    }
}
