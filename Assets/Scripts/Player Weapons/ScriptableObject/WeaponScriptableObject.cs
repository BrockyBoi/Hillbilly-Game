using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Weaponry
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Weapons/New Weapon", order = 1)]
    public class WeaponScriptableObject : ScriptableObject
    {
        public WeaponData WeaponData;
    }
}