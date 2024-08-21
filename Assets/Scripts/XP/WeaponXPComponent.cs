using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Weaponry;

namespace XP
{
    [Serializable]
    public struct PlayerAttributeModifier
    {
        public UpgradeAttribute AttributeType;
        public float AttributeAmount;
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
            if (_owningWeapon.WeaponUpgrades.Count > upgradeIndex)
            {
                Debug.Log("Weapon level: " + (upgradeIndex + 1));
                PlayerAttributeModifier upgradeData = _owningWeapon.WeaponUpgrades[upgradeIndex];
                if (upgradeData.AttributeType == UpgradeAttribute.FireRate)
                {
                    _owningWeapon.WeaponAttributesComponent.DecrementAttributeAmount(upgradeData.AttributeType, upgradeData.IsFlatAmount, upgradeData.AttributeAmount);
                }
                else
                {
                    _owningWeapon.WeaponAttributesComponent.IncrementAttributeAmount(upgradeData.AttributeType, upgradeData.IsFlatAmount, upgradeData.AttributeAmount);
                }
            }
        }
    }
}
