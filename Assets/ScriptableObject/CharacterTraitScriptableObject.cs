using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Weaponry;
using XP;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Traits/New Character Trait", order = 1)]
public class CharacterTraitScriptableObject : ScriptableObject
{
    public string TraitName = "CharacterTrait";
    public CharacterTrait Trait;
}
