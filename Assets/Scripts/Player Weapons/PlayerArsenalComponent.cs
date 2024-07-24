using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerArsenalComponent : MonoBehaviour
{
    [SerializeField]
    int _maxWeaponsAllowed = 5;

    Dictionary<string, PlayerWeapon> _playerWeapons;

    Dictionary<WeaponModifiers, float> _weaponModifiers;

    private void Start()
    {
        _weaponModifiers = new Dictionary<WeaponModifiers, float>()
        { 
            { WeaponModifiers.Speed, 1 }, 
            { WeaponModifiers.Speed, 1 }, 
            { WeaponModifiers.FireRate, 1 }, 
            { WeaponModifiers.Size, 1 } 
        }; 
    }

    public void SetWeaponModiferAmount(WeaponModifiers weaponModifier, float amount)
    {
        _weaponModifiers[weaponModifier] = amount;
    }

    public void IncreaseWeaponModifier(WeaponModifiers weaponModifier, float amount) 
    {
        _weaponModifiers[weaponModifier] += amount;
    }

    public void AddWeapon(PlayerWeapon weapon)
    {
        if (_playerWeapons.Count < _maxWeaponsAllowed && !_playerWeapons.ContainsKey(weapon.WeaponId))
        {
            _playerWeapons.Add(weapon.WeaponId, weapon);
            weapon.Initialize();
        }
    }

    public PlayerWeapon GetWeapon(string weaponId)
    {
        if (_playerWeapons.ContainsKey(weaponId))
        {
            return _playerWeapons[weaponId];
        }

        return null;
    }

    public float GetWeaponModifier(WeaponModifiers weaponModifier) 
    {
        if (!_weaponModifiers.ContainsKey(weaponModifier))
        {
            return 0f;
        }

        return _weaponModifiers[weaponModifier];
    }
}
