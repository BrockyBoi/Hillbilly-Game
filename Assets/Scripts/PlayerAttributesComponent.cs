using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Weaponry
{
    public class PlayerAttributesComponent : MonoBehaviour
    {
        [SerializeField]
        int _maxWeaponsAllowed = 5;

        Dictionary<string, PlayerWeapon> _playerWeapons = new Dictionary<string, PlayerWeapon>();

        Dictionary<UpgradeAttribute, float> _attributes = new Dictionary<UpgradeAttribute, float>();

        public delegate void EOnAttributeChange(UpgradeAttribute attribute, float value);
        public event EOnAttributeChange OnAttributeChanged;

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

        public void SetAttributeValue(UpgradeAttribute weaponModifier, float amount)
        {
            _attributes[weaponModifier] = amount;
        }

        public void IncreaseAttributeValue(UpgradeAttribute weaponModifier, float amount)
        {
            _attributes[weaponModifier] += amount;
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

        public float GetAttribute(UpgradeAttribute attribute)
        {
            if (!_attributes.ContainsKey(attribute))
            {
                float defaultValue = 1;
                if (attribute == UpgradeAttribute.NumberOfEnemiesCanPassThrough || attribute == UpgradeAttribute.NumberOfProjectiles)
                {
                    defaultValue = 0;
                }

                _attributes.Add(attribute, defaultValue);
            }

            return _attributes[attribute];
        }
    }
}
