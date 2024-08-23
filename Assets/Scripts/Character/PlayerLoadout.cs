using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Weaponry;
using XP;

public enum EBasePlayerStats
{
    Health, // Standard Health
    Strength, // Physical Damage and Knockback
    Speed, // Movement Speed
    Agility, // Duration And Dodge Maybe?
    Intelligence, // Tech/Magic Damage And XP Multiplier
    COUNT
}

[System.Serializable]
public class StatModifierData
{
    public int CurrentLevel;
    public CharacterAttributeModifier DefaultModifierPerLevel;
    public SpecialAttributesAtGivenPointData SpecialAttributesAtGivenPointValues;
}

[System.Serializable]
public struct SpecialAttributesAtGivenPointData
{
    public List<CharacterAttributeModifier> AttributeDatas;
    public List<int> IndexOfAttribute;
}

public class PlayerLoadout : MonoBehaviour
{
    public static PlayerLoadout Instance { get; private set; }

    [SerializeField]
    private int _defaultStatLevel = 5;

    [SerializeField]
    private List<WeaponData> _chosenWeapons = new List<WeaponData>();
    public List<WeaponData> ChosenWeapons {  get { return _chosenWeapons; } }

    [SerializeField]
    private List<CharacterTrait> _chosenTraits = new List<CharacterTrait>();
    public List<CharacterTrait> ChosenTraits { get {  return _chosenTraits; } }

    private Dictionary<EBasePlayerStats, StatModifierData> _playerStats = new Dictionary<EBasePlayerStats, StatModifierData>();

    private void Awake()
    {
        Instance = this;

        DontDestroyOnLoad(this);
        for (EBasePlayerStats stat = EBasePlayerStats.Health; stat < EBasePlayerStats.COUNT; stat++)
        {
            _playerStats.Add(stat, new StatModifierData());
            _playerStats[stat].CurrentLevel = _defaultStatLevel;
        }
    }

    public void ToggleWeapon(WeaponData weapon)
    {
        bool hasWeapon = _chosenWeapons.Contains(weapon);
        if (!hasWeapon)
        {
            _chosenWeapons.Add(weapon);
        }
        else
        {
            _chosenWeapons.Remove(weapon);
        }
    }

    public void ToggleTrait(CharacterTrait trait)
    {
        bool hasTrait = _chosenTraits.Contains(trait);
        if (!hasTrait)
        {
            _chosenTraits.Add(trait);
        }
        else
        {
            _chosenTraits.Remove(trait);
        }
    }

    public StatModifierData GetPlayerStat(EBasePlayerStats stat)
    {
        return _playerStats[stat];
    }

    public void IncrementCharacterStatLevel(EBasePlayerStats stat)
    {
        SetCharacterStatLevel(stat, _playerStats[stat].CurrentLevel + 1);
    }

    public void DecreaseCharacterStatLevel(EBasePlayerStats stat)
    {
        SetCharacterStatLevel(stat, _playerStats[stat].CurrentLevel - 1);
    }

    public void SetCharacterStatLevel(EBasePlayerStats stat, int value)
    {
        _playerStats[stat].CurrentLevel = Mathf.Clamp(value, 0, 10);
    }
}
