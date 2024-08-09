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

        private Dictionary<UpgradeAttribute, float> _weaponModifiers = new Dictionary<UpgradeAttribute, float>();

        protected WaitForSeconds _waitForInBetweenProjectiles;

        protected MainPlayer _mainPlayer;

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

            _mainPlayer = MainPlayer.Instance;
            _waitForInBetweenProjectiles = new WaitForSeconds(WeaponData.TimeBetweenProjectiles);

            _weaponXPComponent = GetComponent<WeaponXPComponent>();
            _weaponXPComponent.InitializeWeapon(this);
;
            _projectilePool.Initialize(WeaponData.ProjectilePrefab);

            StartCoroutine(FireWeaponCoroutine());
        }

        IEnumerator FireWeaponCoroutine()
        {
            while (_mainPlayer.IsAlive())
            {
                yield return new WaitForSeconds(2.5f);
                yield return FireWeapon(GetCurrentProjectileData());
                yield return new WaitForSeconds(WeaponData.DefaultFireRate - GetAllAttributeValues(UpgradeAttribute.FireRate));
            }
        }

        protected ProjectileData GetCurrentProjectileData()
        {
            float weaponSpeed = WeaponData.DefaultProjectileData.ProjectileSpeed + GetAllAttributeValues(UpgradeAttribute.ProjectileSpeed);
            float weaponSize = WeaponData.DefaultProjectileData.ProjectileSizeMultiplier + GetAllAttributeValues(UpgradeAttribute.ProjectileSize);
            float damageDealt = WeaponData.DefaultProjectileData.DamageToDeal + GetAllAttributeValues(UpgradeAttribute.Damage);
            float timeBetweenDamage = WeaponData.DefaultProjectileData.TimeBetweenDamage;
            float weaponDuration = WeaponData.DefaultProjectileData.WeaponDuration + GetAllAttributeValues(UpgradeAttribute.Duration);
            int numberOfEnemiesToPassThrough = WeaponData.DefaultProjectileData.NumberOfEnemiesCanPassThrough + (int)GetAllAttributeValues(UpgradeAttribute.NumberOfEnemiesCanPassThrough);
            bool canPassThroughUnlimitedEnemies = WeaponData.DefaultProjectileData.CanPassThroughUnlimitedEnemies;
            bool canArc = WeaponData.DefaultProjectileData.CanWeaponArc;
            int arcCount = WeaponData.DefaultProjectileData.ProjectileArcCount;

            return new ProjectileData(weaponSpeed, weaponSize, damageDealt, timeBetweenDamage, weaponDuration, numberOfEnemiesToPassThrough, canPassThroughUnlimitedEnemies, canArc, arcCount);
        }

        protected float GetAllAttributeValues(UpgradeAttribute upgradeAttribute)
        {
            return WeaponAttributesComponent.GetAttribute(upgradeAttribute) + MainPlayer.Instance.UpgradeAttributesComponent.GetAttribute(upgradeAttribute);
        }

        protected virtual IEnumerator FireWeapon(ProjectileData data)
        {
            int numberOfProjectiles = Mathf.RoundToInt(WeaponData.NumberOfProjectilesToFire + WeaponAttributesComponent.GetAttribute(UpgradeAttribute.NumberOfProjectiles));
            for (int i = 0; i < numberOfProjectiles; i++)
            {
                BaseProjectile projectile = _projectilePool.GetPoolableObject();
                if (projectile)
                {
                    projectile.transform.position = _mainPlayer.transform.position;
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
                else
                {
                    yield break;
                }
            }
        }

        protected virtual Vector3 GetDirectionToFireProjectile()
        {
            return Vector3.zero;
        }
    }
}