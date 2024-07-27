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

            foreach (WeaponUpgradeData upgradeData in _owningWeapon.WeaponData.WeaponUpgrades)
            {
                for (int i = 0; i < upgradeData.AttributeAmounts.Count; i++)
                {
                    if (upgradeData.WeaponAttributes.Count > i && upgradeData.AttributeAmounts.Count > i)
                    {
                        _owningWeapon.ModifyWeapon(upgradeData.WeaponAttributes[i], upgradeData.AttributeAmounts[i]);
                    }
                }
            }
        }
    }
}
