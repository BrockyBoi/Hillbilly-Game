using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StatusEffects
{
    public abstract class StatusEffect
    {
        [SerializeField]
        private StatusEffectData _data;

        public abstract void OnEffectApplied();
        public abstract void OnStackRemoved();
    }
}
