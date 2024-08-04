using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/StatusEffect/New Status Effect", order = 1)]
public class StatusEffectScriptableObject : ScriptableObject
{
    public StatusEffect StatusEffectData;
}