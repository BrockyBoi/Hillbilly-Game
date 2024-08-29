using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using XP;

namespace Weaponry
{
    public enum EBodyWeaponType
    {
        Head, Body, Feet
    }

    [Serializable]
    public struct WeaponData
    {
        [Title("Weapon Name")]
        public string WeaponID;

        [Title("Body Part")]
        public EBodyWeaponType BodyWeaponType;

        [Title("Weapon Stats")]
        [Range(.05f, 20f)]
        public float DefaultFireRate;

        [Range(.05f, 20f)]
        public float MaxFireRate;

        [Range(1, 10)]
        public int NumberOfProjectilesToFire;

        [Range(.05f, 1)]
        public float TimeBetweenProjectiles;

        [Title("Weapon Prefabs")]
        public GameObject WeaponPrefab;

        public GameObject ProjectilePrefab;

        [Title("Projectile Data")]
        public ProjectileData DefaultProjectileData;
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

        protected CharacterAttributesManager _attributesComponent = new CharacterAttributesManager();
        public CharacterAttributesManager WeaponAttributesComponent { get { return _attributesComponent; } }

        [SerializeField, Title("Weapon Upgrades")]
        private List<CharacterAttributeModifier> _weaponUpgrades;
        public List<CharacterAttributeModifier> WeaponUpgrades { get { return _weaponUpgrades; } }

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
            // Base Stats
            float weaponSpeed =  GetAllAttributeValues(UpgradeAttribute.ProjectileSpeed, WeaponData.DefaultProjectileData.ProjectileSpeed);
            float weaponSize =  GetAllAttributeValues(UpgradeAttribute.ProjectileSize, WeaponData.DefaultProjectileData.ProjectileSizeMultiplier);
            float damageAmount = GetAllAttributeValues(UpgradeAttribute.Damage, WeaponData.DefaultProjectileData.DamageToDeal);
            float timeBetweenDamage = WeaponData.DefaultProjectileData.TimeBetweenDamage;

            // Duration
            float weaponDuration = GetAllAttributeValues(UpgradeAttribute.Duration, WeaponData.DefaultProjectileData.WeaponDuration);

            // Knockback
            bool canKnockBack = WeaponData.DefaultProjectileData.CanKnockback;
            float knockbackAmount = GetAllAttributeValues(UpgradeAttribute.KnockbackMultiplier, WeaponData.DefaultProjectileData.KnockbackAmount);
            float knockBackTiming = WeaponData.DefaultProjectileData.KnockbackTime;
            bool canKnockbackIntoOtherEnemies = WeaponData.DefaultProjectileData.CanKnockbackIntoOtherEnemies;


            // Pass through
            int numberOfEnemiesToPassThrough = (int)GetAllAttributeValues(UpgradeAttribute.NumberOfEnemiesCanPassThrough, WeaponData.DefaultProjectileData.NumberOfEnemiesCanPassThrough);
            bool canPassThroughUnlimitedEnemies = WeaponData.DefaultProjectileData.CanPassThroughUnlimitedEnemies;

            // Arc
            bool canArc = WeaponData.DefaultProjectileData.CanWeaponArc;
            int arcCount = WeaponData.DefaultProjectileData.ProjectileArcCount;

            // Explosion
            bool canExplode = WeaponData.DefaultProjectileData.CanProjectileExplode;
            GameObject explosionPrefab = WeaponData.DefaultProjectileData.ExplosionPrefab;

            return new ProjectileData(weaponSpeed, weaponSize, canKnockBack,
                knockbackAmount, knockBackTiming, canKnockbackIntoOtherEnemies, damageAmount, 
                timeBetweenDamage, weaponDuration, numberOfEnemiesToPassThrough, 
                canPassThroughUnlimitedEnemies, canArc, arcCount,
                canExplode, explosionPrefab);
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