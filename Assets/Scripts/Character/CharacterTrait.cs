using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XP;

[Serializable]
public class CharacterTrait
{
    [SerializeField]
    private List<CharacterAttributeModifier> _characterAttributes;
    public List<CharacterAttributeModifier> CharacterAttributes { get { return _characterAttributes; } }
}
