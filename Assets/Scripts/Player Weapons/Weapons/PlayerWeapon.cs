using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using XP;

namespace Weaponry
{
    [Serializable]
    public struct WeaponData
    {
        public string WeaponID;

        [Range(.05f, 20f)]
        public float DefaultFireRate;

        [Range(.05f, 20f)]
        public float MaxFireRate;

        [Range(1, 10)]
        public int NumberOfProjectilesToFire;

        [Range(.05f, 1)]
        public float TimeBetweenProjectiles;

        public GameObject ProjectilePrefab;

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

        protected WaitForSeconds _waitForInBetweenProjectiles;

        protected MainPlayer _owningPlayer;
        public MainPlayer OwningPlayer { get { return _owningPlayer; } }

        protected PoolableObjectsComponent<BaseProjectile> _projectilePool = new PoolableObjectsComponent<BaseProjectile>();
        public PoolableObjectsComponent<BaseProjectile> ProjectilePool { get { return _projectilePool; } }

        protected UpgradeAttributesComponent _attributesComponent = new UpgradeAttributesComponent();
        public UpgradeAttributesComponent WeaponAttributesComponent { get { return _attributesComponent; } }

        protected virtual void OnEnable()
        {

        }

        protected virtual void OnDisable()
        {

        }

        public void Initialize()
        {
            if (!_weaponScriptableObject)
            {
                Debug.LogError("Needs weapon scriptable object");
                return;
            }

            _owningPlayer = MainPlayer.Instance;
            _waitForInBetweenProjectiles = new WaitForSeconds(WeaponData.TimeBetweenProjectiles);

            _weaponXPComponent = GetComponent<WeaponXPComponent>();
            _weaponXPComponent.InitializeWeapon(this);
;
            _projectilePool.Initialize(WeaponData.ProjectilePrefab);

            StartCoroutine(FireWeaponCoroutine());
        }

        IEnumerator FireWeaponCoroutine()
        {
            yield return new WaitForSeconds(2.5f);

            while (_owningPlayer.IsAlive())
            {
                yield return FireWeapon(GetCurrentProjectileData());
                yield return new WaitForSeconds(GetAllAttributeValues(UpgradeAttribute.FireRate, WeaponData.DefaultFireRate, false));
            }
        }

        protected ProjectileData GetCurrentProjectileData()
        {
            float weaponSpeed =  GetAllAttributeValues(UpgradeAttribute.ProjectileSpeed, WeaponData.DefaultProjectileData.ProjectileSpeed);
            float weaponSize =  GetAllAttributeValues(UpgradeAttribute.ProjectileSize, WeaponData.DefaultProjectileData.ProjectileSizeMultiplier);
            float damageAmount = GetAllAttributeValues(UpgradeAttribute.Damage, WeaponData.DefaultProjectileData.DamageToDeal);
            float knockbackAmount = GetAllAttributeValues(UpgradeAttribute.KnockbackMultiplier, WeaponData.DefaultProjectileData.KnockbackAmount);
            bool canKnockbackIntoOtherEnemies = WeaponData.DefaultProjectileData.CanKnockbackIntoOtherEnemies;
            float timeBetweenDamage = WeaponData.DefaultProjectileData.TimeBetweenDamage;
            float weaponDuration =  GetAllAttributeValues(UpgradeAttribute.Duration, WeaponData.DefaultProjectileData.WeaponDuration);
            int numberOfEnemiesToPassThrough = (int)GetAllAttributeValues(UpgradeAttribute.NumberOfEnemiesCanPassThrough, WeaponData.DefaultProjectileData.NumberOfEnemiesCanPassThrough);
            bool canPassThroughUnlimitedEnemies = WeaponData.DefaultProjectileData.CanPassThroughUnlimitedEnemies;
            bool canArc = WeaponData.DefaultProjectileData.CanWeaponArc;
            int arcCount = WeaponData.DefaultProjectileData.ProjectileArcCount;

            return new ProjectileData(weaponSpeed, weaponSize, knockbackAmount, canKnockbackIntoOtherEnemies, damageAmount, timeBetweenDamage, weaponDuration, numberOfEnemiesToPassThrough, canPassThroughUnlimitedEnemies, canArc, arcCount);
        }

        protected float GetAllAttributeValues(UpgradeAttribute upgradeAttribute, float defaultValue, bool shouldIncrement = true)
        {
            float flatValues = WeaponAttributesComponent.GetFlatAttributeValue(upgradeAttribute) + _owningPlayer.UpgradeAttributesComponent.GetFlatAttributeValue(upgradeAttribute);
            float multiplierValues = WeaponAttributesComponent.GetMultiplierAttributeValue(upgradeAttribute) * _owningPlayer.UpgradeAttributesComponent.GetMultiplierAttributeValue(upgradeAttribute);
            return (shouldIncrement ? defaultValue + flatValues : defaultValue - flatValues) * multiplierValues;
        }

        protected virtual IEnumerator FireWeapon(ProjectileData data)
        {
            int numberOfProjectiles = Mathf.RoundToInt(WeaponAttributesComponent.GetModifiedAttributeValue(UpgradeAttribute.NumberOfProjectiles, WeaponData.NumberOfProjectilesToFire));
            for (int i = 0; i < numberOfProjectiles; i++)
            {
                BaseProjectile projectile = _projectilePool.GetPoolableObject();
                if (projectile)
                {
                    projectile.transform.position = _owningPlayer.transform.position;
                    projectile.InitializeProjectile(this, data, GetDirectionToFireProjectile());
                }
                else
                {
                    Debug.LogError("Projectile is null");
                }

                if (numberOfProjectiles > 1)
                {
                    yield return _waitForInBetweenProjectiles;
                }
            }

            if (data.WeaponDuration > 0)
            {
                yield return new WaitForSeconds(data.WeaponDuration);
            }
        }

        protected virtual Vector3 GetDirectionToFireProjectile()
        {
            return Vector3.zero;
        }
    }
}