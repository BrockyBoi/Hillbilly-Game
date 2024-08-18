using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Weaponry;

public enum UpgradeAttribute
{
    ProjectileSpeed, NumberOfProjectiles, ProjectileSize, FireRate, NumberOfEnemiesCanPassThrough, Damage, Duration, PickupRange, MovementSpeed, ProjectileArcCount, XPMultiplier, MaxHealthMultiplier, AllStatusEffectDamageMultiplier, KnockbackMultiplier
}

public class PlayerWeaponsComponent : MonoBehaviour
{
    [SerializeField]
    int _maxWeaponsAllowed = 5;

    Dictionary<string, PlayerWeapon> _playerWeapons = new Dictionary<string, PlayerWeapon>();

    private void Start()
    {
        foreach (PlayerWeapon weapon in GetComponentsInChildren<PlayerWeapon>())
        {
            if (weapon)
            {
                AddWeapon(weapon);
            }
        }
    }

    public void AddWeapon(PlayerWeapon weapon)
    {
        if (_playerWeapons.Count < _maxWeaponsAllowed && weapon.WeaponScriptableObject && !_playerWeapons.ContainsKey(weapon.WeaponData.WeaponID))
        {
            _playerWeapons.Add(weapon.WeaponData.WeaponID, weapon);
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
}
