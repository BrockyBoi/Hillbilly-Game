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
        public List<SingleUpgradeData> WeaponAttributes;
    }

    [Serializable]
    public struct SingleUpgradeData
    {
        public UpgradeAttribute Attribute;
        public float UpgradeAmount;
        public bool IsFlatAmount;
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
                for (int i = 0; i < upgradeData.WeaponAttributes.Count; i++)
                {
                    SingleUpgradeData singleUpgrade = upgradeData.WeaponAttributes[i];
                    if (singleUpgrade.Attribute == UpgradeAttribute.FireRate)
                    {
                        _owningWeapon.WeaponAttributesComponent.DecrementAttributeAmount(singleUpgrade.Attribute, singleUpgrade.IsFlatAmount, singleUpgrade.UpgradeAmount);
                    }
                    else
                    {
                        _owningWeapon.WeaponAttributesComponent.IncrementAttributeAmount(singleUpgrade.Attribute, singleUpgrade.IsFlatAmount, singleUpgrade.UpgradeAmount);
                    }
                }
            }
        }
    }
}
