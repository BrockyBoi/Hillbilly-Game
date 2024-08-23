using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Weaponry
{
    public class WeaponsDataManager : MonoBehaviour
    {
        public static WeaponsDataManager Instance { get; private set; }

        [SerializeField]
        private List<WeaponScriptableObject> _allWeaponsData;

        public List<WeaponScriptableObject> AllWeaponsData { get { return _allWeaponsData; } }

        private void Awake()
        {
            Instance = this;

            DontDestroyOnLoad(this);
        }
    }
}
