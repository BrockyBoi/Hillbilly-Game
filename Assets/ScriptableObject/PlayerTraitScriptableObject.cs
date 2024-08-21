using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Weaponry;
using XP;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Traits/New Player Trait", order = 1)]
public class PlayerTraitScriptableObject : ScriptableObject
{
    public string TraitName = "PlayerTrait";
    public List<PlayerAttributeModifier> PlayerUpgrades;
}
