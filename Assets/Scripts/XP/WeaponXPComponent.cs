using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Weaponry;

namespace XP
{
    [Serializable]
    public struct WeaponUpgradeData
    {
        public List<UpgradeAttribute> WeaponAttributes;
        public List<float> AttributeAmounts;
    }


    public class WeaponXPComponent : XPComponent
    {
        private PlayerWeapon _owningWeapon;

        public void InitializeWeapon(PlayerWeapon owner)
        {
            _owningWeapon = owner;
        }

        public override void AddXP(float xp)
        {
            base.AddXP(xp);
        }

        protected override void LevelUp()
        {
            base.LevelUp();

            int upgradeIndex = _currentLevel - 1;
            if (_owningWeapon.WeaponData.WeaponUpgrades.Count > upgradeIndex)
            {
                Debug.Log("Weapon level: " + (upgradeIndex + 1));
                WeaponUpgradeData upgradeData = _owningWeapon.WeaponData.WeaponUpgrades[upgradeIndex];
                for (int i = 0; i < upgradeData.AttributeAmounts.Count; i++)
                {
                    if (upgradeData.WeaponAttributes.Count > i && upgradeData.AttributeAmounts.Count > i)
                    {
                        UpgradeAttribute attribute = upgradeData.WeaponAttributes[i];
                        if (attribute == UpgradeAttribute.FireRate)
                        {
                            _owningWeapon.WeaponAttributesComponent.DecrementAttribute(attribute, upgradeData.AttributeAmounts[i]);
                        }
                        else
                        {
                            _owningWeapon.WeaponAttributesComponent.IncrementAttribute(attribute, upgradeData.AttributeAmounts[i]);
                        }
                    }
                }
            }
        }
    }
}
