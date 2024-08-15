using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StatusEffects;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/StatusEffect/New Status Effect", order = 1)]
public class StatusEffectScriptableObject : ScriptableObject
{
    public StatusEffectData StatusEffectData;
}