using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StatusEffects
{
    [System.Serializable]
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
        private List<StatusEffectScriptableObject> _allStatusEffectDatas;

        private Dictionary<EStatusEffectType, StatusEffectManagerData> _statusEffects = new Dictionary<EStatusEffectType, StatusEffectManagerData>();

        public delegate void EOnStackLoss(StatusEffectData effectData);
        public event EOnStackLoss OnStackLoss;

        private Character _owningCharacter;
        
        private void Awake()
        {
            InitializeDictionary();

            _owningCharacter = GetComponent<Character>();
            if (!_owningCharacter)
            {
                Debug.LogError("Status effects manager does not have associated Character");
            }
        }

        public int GetStacks(EStatusEffectType statusEffectType)
        {
            if (!_statusEffects.ContainsKey(statusEffectType))
            {
                InitializeDictionary();
            }

            return _statusEffects[statusEffectType].CurrentStacks;
        }

        public void IncrementStacks(EStatusEffectType statusEffectType, int stacks)
        {
            SetStacks(statusEffectType, _statusEffects[statusEffectType].CurrentStacks + stacks);
        }

        public void ClearStacks(EStatusEffectType statusEffectType)
        {
            SetStacks(statusEffectType, 0);
        }

        private void InitializeDictionary()
        {
            foreach (StatusEffectScriptableObject effect in _allStatusEffectDatas)
            {
                if (!_statusEffects.ContainsKey(effect.StatusEffectData.StatusEffectType))
                {
                    StatusEffectManagerData managerData = new StatusEffectManagerData(effect.StatusEffectData);
                    if (effect.StatusEffectData.TimeToLoseStack > 0)
                    {
                        managerData.StatusEffectTimerCoroutine = LoseStack(effect.StatusEffectData.StatusEffectType);
                    }
                    _statusEffects.Add(effect.StatusEffectData.StatusEffectType, managerData);
                }
            }
        }

        public void ClearAllStacks()
        {
            foreach (var keyValuePair in _statusEffects)
            {
                if (_statusEffects.ContainsKey(keyValuePair.Key))
                {
                    ClearStacks(keyValuePair.Key);
                }
            }
        }

        private void SetStacks(EStatusEffectType statusEffectType, int stacks)
        {
            StatusEffectManagerData data = _statusEffects[statusEffectType];

            int prevStacks = data.CurrentStacks;
            data.CurrentStacks = Mathf.Clamp(stacks, 0, data.StatusEffectData.MaxStacks);
        
            if (prevStacks > 0 && data.CurrentStacks == 0)
            {
                StopCoroutine(data.StatusEffectTimerCoroutine);
            }
            else if (prevStacks == 0 && data.CurrentStacks > 0 && _owningCharacter.IsAlive())
            {
                StartCoroutine(data.StatusEffectTimerCoroutine);
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

                OnStackLoss?.Invoke(managerData.StatusEffectData);

                SetStacks(statusEffectType, managerData.CurrentStacks - 1);

                if (managerData.StatusEffectData.DamagePerStack > 0)
                {
                    _owningCharacter.HealthComponent.DoDamage(managerData.StatusEffectData.DamagePerStack * (GetStacks(statusEffectType) + 1));
                }
            }
        }
    }
}
