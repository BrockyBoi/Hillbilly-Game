using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XP;

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

        [Range(.05f, 20f)]
        public float MaxFireRate;

        [Range(1, 10)]
        public int NumberOfProjectilesToFire;
        [Range(.05f, 1)]
        public float TimeBetweenProjectiles;

        public Projectile ProjectilePrefab;

        public ProjectileData DefaultProjectileData;

        public List<WeaponUpgradeData> WeaponUpgrades;
    }

    public abstract class PlayerWeapon : MonoBehaviour
    {
        [Title("Weapon Properties")]
        [SerializeField]
        protected WeaponScriptableObject _weaponScriptableObject;
        public WeaponScriptableObject WeaponScriptableObject { get { return _weaponScriptableObject; } }
        public WeaponData WeaponData { get { return _weaponScriptableObject.WeaponData; } }

        private Dictionary<WeaponAttribute, float> _weaponModifiers = new Dictionary<WeaponAttribute, float>();

        private WaitForSeconds _waitForNextFire;
        protected WaitForSeconds _waitForInBetweenProjectiles;

        private MainPlayer _mainPlayer;

        public void Initialize()
        {
            if (!_weaponScriptableObject)
            {
                Debug.LogError("Needs weapon scriptable object");
                return;
            }

            _mainPlayer = MainPlayer.Instance;
            _waitForNextFire = new WaitForSeconds(WeaponData.DefaultFireRate);
            _waitForInBetweenProjectiles = new WaitForSeconds(WeaponData.TimeBetweenProjectiles);
            StartCoroutine(FireWeaponCoroutine());
        }

        IEnumerator FireWeaponCoroutine()
        {
            while (_mainPlayer.IsAlive())
            {
                yield return _waitForNextFire;                

                yield return FireWeapon(GetCurrentProjectileData());
            }
        }

        protected float GetWeaponModifier(WeaponAttribute weaponModifier)
        {
            if (!_weaponModifiers.ContainsKey(weaponModifier))
            {
                float defaultValue = 0;
                if (weaponModifier == WeaponAttribute.Size)
                {
                    defaultValue = 1;
                }

                _weaponModifiers.Add(weaponModifier, defaultValue);
            }

            return _weaponModifiers[weaponModifier] + _mainPlayer.ArsenalComponent.GetWeaponModifier(weaponModifier);
        }

        protected void SetWeaponModifier(WeaponAttribute weaponModifier, float newValue)
        {
            if (!_weaponModifiers.ContainsKey(weaponModifier))
            {
                float defaultValue = 0;
                if (weaponModifier == WeaponAttribute.Size)
                {
                    defaultValue = 1;
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

        protected ProjectileData GetCurrentProjectileData()
        {
            float weaponSpeed = WeaponData.DefaultProjectileData.ProjectileSpeed + GetWeaponModifier(WeaponAttribute.Speed);
            float weaponSize = WeaponData.DefaultProjectileData.ProjectileSizeMultiplier + GetWeaponModifier(WeaponAttribute.Size);
            float damageDealt = WeaponData.DefaultProjectileData.DamageToDeal + GetWeaponModifier(WeaponAttribute.Damage);
            float timeBetweenDamage = WeaponData.DefaultProjectileData.TimeBetweenDamage;
            float weaponDuration = WeaponData.DefaultProjectileData.WeaponDuration + GetWeaponModifier(WeaponAttribute.Duration);
            int numberOfEnemiesToPassThrough = WeaponData.DefaultProjectileData.NumberOfEnemiesCanPassThrough + (int)GetWeaponModifier(WeaponAttribute.NumberOfEnemiesCanPassThrough);
            bool canPassThroughUnlimitedEnemies = WeaponData.DefaultProjectileData.CanPassThroughUnlimitedEnemies;

            ProjectileData projectileData = new ProjectileData(weaponSpeed, weaponSize, damageDealt, timeBetweenDamage, weaponDuration, numberOfEnemiesToPassThrough, canPassThroughUnlimitedEnemies);
            return projectileData;
        }

        protected abstract IEnumerator FireWeapon(ProjectileData projectileData);
    }
}