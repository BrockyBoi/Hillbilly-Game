using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Weaponry
{
    public class PlayerArsenalComponent : MonoBehaviour
    {
        [SerializeField]
        int _maxWeaponsAllowed = 5;

        Dictionary<string, PlayerWeapon> _playerWeapons = new Dictionary<string, PlayerWeapon>();

        Dictionary<WeaponAttribute, float> _weaponModifiers = new Dictionary<WeaponAttribute, float>();

        private void Start()
        {
            foreach (PlayerWeapon weapon in GetComponents<PlayerWeapon>())
            {
                if (weapon)
                {
                    AddWeapon(weapon);
                }
            }
        }

        public void SetWeaponModiferAmount(WeaponAttribute weaponModifier, float amount)
        {
            _weaponModifiers[weaponModifier] = amount;
        }

        public void IncreaseWeaponModifier(WeaponAttribute weaponModifier, float amount)
        {
            _weaponModifiers[weaponModifier] += amount;
        }

        public void AddWeapon(PlayerWeapon weapon)
        {
            if (_playerWeapons.Count < _maxWeaponsAllowed && !_playerWeapons.ContainsKey(weapon.WeaponData.WeaponID))
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

        public float GetWeaponModifier(WeaponAttribute weaponModifier)
        {
            if (!_weaponModifiers.ContainsKey(weaponModifier))
            {
                float defaultValue = 1;
                if (weaponModifier == WeaponAttribute.NumberOfEnemiesCanPassThrough || weaponModifier == WeaponAttribute.NumberOfProjectiles)
                {
                    defaultValue = 0;
                }

                _weaponModifiers.Add(weaponModifier, defaultValue);
            }

            return _weaponModifiers[weaponModifier];
        }
    }
}
