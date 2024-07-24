using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Weaponry
{
    public enum WeaponAttribute
    {
        Speed, NumberOfProjectiles, Size, FireRate, NumberOfEnemiesCanPassThrough, Damage, Duration
    }

    [Serializable]
    public struct WeaponData
    {
        public string WeaponID;
        public float DefaultFireRate;
        public float MaxFireRate;

        public ProjectileData DefaultProjectileData;
    }

    public abstract class PlayerWeapon : MonoBehaviour
    {
        [Title("Weapon Properties")]
        [SerializeField]
        protected WeaponData _weaponData;
        public WeaponData WeaponData { get { return _weaponData; } }

        Dictionary<WeaponAttribute, float> _weaponModifiers = new Dictionary<WeaponAttribute, float>();

        [Title("Prefab")]
        [SerializeField]
        protected Projectile _projectilePrefab;

        private WaitForSeconds _waitForNextFire;

        private MainPlayer _mainPlayer;

        public void Initialize()
        {
            _mainPlayer = MainPlayer.Instance;
            _waitForNextFire = new WaitForSeconds(WeaponData.DefaultFireRate);
            StartCoroutine(FireWeaponCoroutine());
        }

        IEnumerator FireWeaponCoroutine()
        {
            while (_mainPlayer.IsAlive())
            {
                yield return _waitForNextFire;

                float weaponSpeed = WeaponData.DefaultProjectileData.ProjectileSpeed + GetWeaponModifier(WeaponAttribute.Speed);
                float weaponSize = WeaponData.DefaultProjectileData.ProjectileSizeMultiplier + GetWeaponModifier(WeaponAttribute.Size);
                float damageDealt = WeaponData.DefaultProjectileData.DamageToDeal + GetWeaponModifier(WeaponAttribute.Damage);
                float timeBetweenDamage = WeaponData.DefaultProjectileData.TimeBetweenDamage;
                float weaponDuration = WeaponData.DefaultProjectileData.WeaponDuration + GetWeaponModifier(WeaponAttribute.Duration);
                int numberOfEnemiesToPassThrough = WeaponData.DefaultProjectileData.NumberOfEnemiesCanPassThrough + (int)GetWeaponModifier(WeaponAttribute.NumberOfEnemiesCanPassThrough);
                bool canPassThroughUnlimitedEnemies = WeaponData.DefaultProjectileData.CanPassThroughUnlimitedEnemies;
                FireWeapon(new ProjectileData(weaponSpeed, weaponSize, damageDealt, timeBetweenDamage, weaponDuration, numberOfEnemiesToPassThrough, canPassThroughUnlimitedEnemies));
            }
        }

        protected float GetWeaponModifier(WeaponAttribute weaponModifier)
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

            return _weaponModifiers[weaponModifier] + _mainPlayer.ArsenalComponent.GetWeaponModifier(weaponModifier);
        }

        protected void SetWeaponModifier(WeaponAttribute weaponModifier, float newValue)
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

            _weaponModifiers[weaponModifier] = newValue;
        }

        public void ModifyWeapon(WeaponAttribute modifier, float changeAmount)
        {
            switch (modifier)
            {
                case WeaponAttribute.FireRate:
                    {
                        SetWeaponModifier(modifier, GetWeaponModifier(modifier) - changeAmount);
                        _waitForNextFire = new WaitForSeconds(Mathf.Clamp(WeaponData.DefaultFireRate - GetWeaponModifier(WeaponAttribute.FireRate), WeaponData.MaxFireRate, WeaponData.DefaultFireRate));
                        break;
                    }

                default:
                    {
                        SetWeaponModifier(modifier, GetWeaponModifier(modifier) + changeAmount);
                        break;
                    }
            }
        }

        protected abstract void FireWeapon(ProjectileData projectileData);
    }
}