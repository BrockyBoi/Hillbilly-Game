using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Weaponry;

namespace StatusEffects
{
    public class StatusEffectsDataManager : MonoBehaviour
    {
        public static StatusEffectsDataManager Instance { get; private set; }

        [SerializeField]
        private List<StatusEffectScriptableObject> _allStatusEffects;

        public List<StatusEffectScriptableObject> AllStatusEffectsData { get { return _allStatusEffects; } }

        private void Awake()
        {
            Instance = this;

            DontDestroyOnLoad(this);
        }
    }
}
