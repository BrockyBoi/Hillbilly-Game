using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StatusEffects
{
    class StatusEffectManagerData
    {
        public StatusEffectData StatusEffectData;
        public int CurrentStacks;
        public IEnumerator StatusEffectTimerCoroutine;

        public StatusEffectManagerData(StatusEffectData statusEffect)
        {
            StatusEffectData = statusEffect;
            CurrentStacks = 0;
            StatusEffectTimerCoroutine = null;
        }
    }

    public class StatusEffectsManager : MonoBehaviour
    {
        [SerializeField]
        private List<StatusEffectData> _allStatusEffectDatas;

        private Dictionary<EStatusEffectType, StatusEffectManagerData> _statusEffects = new Dictionary<EStatusEffectType, StatusEffectManagerData>();

        public delegate void EOnStackLoss(StatusEffectData effectData);
        public event EOnStackLoss OnStackLoss;
        
        private void Awake()
        {
            foreach (StatusEffectData effect in _allStatusEffectDatas)
            {
                if (_statusEffects.ContainsKey(effect.StatusEffectType))
                {
                    StatusEffectManagerData managerData = new StatusEffectManagerData(effect);
                    if (effect.TimeToLoseStack > 0)
                    {
                        managerData.StatusEffectTimerCoroutine = LoseStack(effect.StatusEffectType);
                    }
                    _statusEffects.Add(effect.StatusEffectType, managerData);
                }
            }
        }

        public void IncrementStacks(EStatusEffectType statusEffectType, int stacks)
        {
            int prevStacks = _statusEffects[statusEffectType].CurrentStacks;
            SetStacks(statusEffectType, _statusEffects[statusEffectType].CurrentStacks + stacks);

            if (prevStacks <= 0)
            {
                StartCoroutine(_statusEffects[statusEffectType].StatusEffectTimerCoroutine);
            }
        }

        public void ClearStacks(EStatusEffectType statusEffectType)
        {
            SetStacks(statusEffectType, 0);
        }

        private void SetStacks(EStatusEffectType statusEffectType, int stacks)
        {
            _statusEffects[statusEffectType].CurrentStacks = Mathf.Clamp(stacks, 0, _statusEffects[statusEffectType].StatusEffectData.MaxStacks);
        
            if (_statusEffects[statusEffectType].CurrentStacks <= 0)
            {
                StopCoroutine(_statusEffects[statusEffectType].StatusEffectTimerCoroutine);
            }
        }

        IEnumerator LoseStack(EStatusEffectType statusEffectType)
        {
            if (!_statusEffects.ContainsKey(statusEffectType))
            {
                yield break;
            }

            StatusEffectManagerData managerData = _statusEffects[statusEffectType];
            while (managerData.CurrentStacks > 0)
            {
                yield return new WaitForSeconds(managerData.StatusEffectData.TimeToLoseStack);

                OnStackLoss.Invoke(managerData.StatusEffectData);

                SetStacks(statusEffectType, managerData.CurrentStacks - 1);
            }
        }
    }
}
