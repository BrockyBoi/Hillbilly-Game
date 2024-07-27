using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XP;

namespace Weaponry
{
    public enum UpgradeAttribute
    {
        ProjectileSpeed, NumberOfProjectiles, Size, FireRate, NumberOfEnemiesCanPassThrough, Damage, Duration, PickupRange, MovementSpeed
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

    [RequireComponent(typeof(WeaponXPComponent))]
    public abstract class PlayerWeapon : MonoBehaviour
    {
        [Title("Weapon Properties")]
        [SerializeField]
        protected WeaponScriptableObject _weaponScriptableObject;
        public WeaponScriptableObject WeaponScriptableObject { get { return _weaponScriptableObject; } }

        protected WeaponXPComponent _weaponXPComponent;
        public WeaponXPComponent WeaponXPComponent { get { return _weaponXPComponent; } }
        public WeaponData WeaponData { get { return _weaponScriptableObject.WeaponData; } }

        private Dictionary<UpgradeAttribute, float> _weaponModifiers = new Dictionary<UpgradeAttribute, float>();

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
            _weaponXPComponent = GetComponent<WeaponXPComponent>();
            _weaponXPComponent.InitializeWeapon(this);
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

        protected float GetWeaponModifier(UpgradeAttribute weaponModifier)
        {
            if (!_weaponModifiers.ContainsKey(weaponModifier))
            {
                float defaultValue = 0;
                if (weaponModifier == UpgradeAttribute.Size)
                {
                    defaultValue = 1;
                }

                _weaponModifiers.Add(weaponModifier, defaultValue);
            }

            return _weaponModifiers[weaponModifier] + _mainPlayer.AttributesComponent.GetAttribute(weaponModifier);
        }

        protected void SetWeaponModifier(UpgradeAttribute weaponModifier, float newValue)
        {
            if (!_weaponModifiers.ContainsKey(weaponModifier))
            {
                float defaultValue = 0;
                if (weaponModifier == UpgradeAttribute.Size)
                {
                    defaultValue = 1;
                }

                _weaponModifiers.Add(weaponModifier, defaultValue);
            }

            _weaponModifiers[weaponModifier] = newValue;
        }

        public void ModifyWeapon(UpgradeAttribute modifier, float changeAmount)
        {
            switch (modifier)
            {
                case UpgradeAttribute.FireRate:
                    {
                        SetWeaponModifier(modifier, GetWeaponModifier(modifier) - changeAmount);
                        _waitForNextFire = new WaitForSeconds(Mathf.Clamp(WeaponData.DefaultFireRate - GetWeaponModifier(UpgradeAttribute.FireRate), WeaponData.MaxFireRate, WeaponData.DefaultFireRate));
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
            float weaponSpeed = WeaponData.DefaultProjectileData.ProjectileSpeed + GetWeaponModifier(UpgradeAttribute.ProjectileSpeed);
            float weaponSize = WeaponData.DefaultProjectileData.ProjectileSizeMultiplier + GetWeaponModifier(UpgradeAttribute.Size);
            float damageDealt = WeaponData.DefaultProjectileData.DamageToDeal + GetWeaponModifier(UpgradeAttribute.Damage);
            float timeBetweenDamage = WeaponData.DefaultProjectileData.TimeBetweenDamage;
            float weaponDuration = WeaponData.DefaultProjectileData.WeaponDuration + GetWeaponModifier(UpgradeAttribute.Duration);
            int numberOfEnemiesToPassThrough = WeaponData.DefaultProjectileData.NumberOfEnemiesCanPassThrough + (int)GetWeaponModifier(UpgradeAttribute.NumberOfEnemiesCanPassThrough);
            bool canPassThroughUnlimitedEnemies = WeaponData.DefaultProjectileData.CanPassThroughUnlimitedEnemies;

            ProjectileData projectileData = new ProjectileData(weaponSpeed, weaponSize, damageDealt, timeBetweenDamage, weaponDuration, numberOfEnemiesToPassThrough, canPassThroughUnlimitedEnemies);
            return projectileData;
        }

        protected abstract IEnumerator FireWeapon(ProjectileData projectileData);
    }
}