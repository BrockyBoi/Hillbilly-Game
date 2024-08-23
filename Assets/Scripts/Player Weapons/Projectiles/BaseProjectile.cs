using Sirenix.OdinInspector;
using StatusEffects;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XP;

namespace Weaponry
{
    [Serializable]
    public struct ProjectileData
    {
        [Title("Base Stats")]
        [Range(1, 10)]
        public float ProjectileSpeed;

        [Range(1, 5)]
        public float ProjectileSizeMultiplier;

        [Range(0, 1000)]
        public float DamageToDeal;

        [Range(.01f, 10)]
        public float TimeBetweenDamage;

        [Range(0f, 10)]
        public float WeaponDuration;

        [Title("Knockback")]
        public bool CanKnockback;

        [ShowIf("CanKnockback")]
        [Range(0, 100)]
        public float KnockbackAmount;

        [ShowIf("CanKnockback")]
        [Range(0, 5)]
        public float KnockbackTime;

        [ShowIf("CanKnockback")]
        public bool CanKnockbackIntoOtherEnemies;

        [Title("Pass Through Enemies")]
        [Range(0, 10)]
        public int NumberOfEnemiesCanPassThrough;

        public bool CanPassThroughUnlimitedEnemies;

        [Title("Weapon Arc")]
        public bool CanWeaponArc;

        [ShowIf("CanWeaponArc")]
        [Range(0, 50)]
        public int ProjectileArcCount;

        public ProjectileData(float speed, float sizeMultiplier, bool canKnockBack, float knockbackAmount, float knockBackTiming, bool canKnockbackIntoOtherEnemies, float damageToDeal, float timeBetweenDamage, float weaponDuration, int numberOfEnemiesToPassThrough, bool canPassThroughUnlimitedEnemies, bool canWeaponArc, int arcCount)
        {
            this.ProjectileSpeed = speed;
            this.ProjectileSizeMultiplier = sizeMultiplier;
            this.DamageToDeal = damageToDeal;
            this.WeaponDuration = weaponDuration;
            this.TimeBetweenDamage = timeBetweenDamage;
            this.NumberOfEnemiesCanPassThrough = numberOfEnemiesToPassThrough;
            this.CanPassThroughUnlimitedEnemies = canPassThroughUnlimitedEnemies;
            this.CanWeaponArc = canWeaponArc;
            this.ProjectileArcCount = arcCount;

            // Knockback
            this.KnockbackAmount = knockbackAmount;
            this.CanKnockbackIntoOtherEnemies = canKnockbackIntoOtherEnemies;
            this.KnockbackTime = knockBackTiming;
            this.CanKnockback = canKnockBack;
        }
    }

    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class BaseProjectile : MonoBehaviour, IPoolableObject
    {
        protected ProjectileData _projectileData;
        protected PlayerWeapon _weapon;
        
        private int _numberOfEnemiesPassedThrough = 0;
        protected Vector3 _direction = Vector3.zero;
        WaitForSeconds _waitForTimeBetweenDamage;
        List<Enemy> _enemiesInContactWith = new List<Enemy>();

        private SpriteRenderer _spriteRenderer;
        protected BoxCollider2D _boxCollider;

        [Title("Status Effects")]
        [SerializeField]
        protected List<StatusEffectAttackData> _statusEffectsToApply;

        [Title("Base Projectile Data")]
        [SerializeField]
        private float _maxDistanceFromPlayerBeforeDestroy = 15;

        private Vector3 _initialScale = Vector3.zero;

        IEnumerator _damageAllEnemiesInRange;

        protected bool _initialized = false;

        protected virtual void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _boxCollider = GetComponent<BoxCollider2D>();
        }

        protected virtual void OnEnable()
        {

        }

        protected virtual void OnDisable()
        {

        }

        protected virtual void Start()
        {
            _damageAllEnemiesInRange = DamageAllEnemiesInRange();
            _waitForTimeBetweenDamage = new WaitForSeconds(_projectileData.TimeBetweenDamage);
        }

        private void Update()
        {
            if (_initialized)
            {
                MoveProjectile();

                float distanceFromPlayer = (transform.position - _weapon.OwningPlayer.transform.position).sqrMagnitude;
                if (distanceFromPlayer > _maxDistanceFromPlayerBeforeDestroy)
                {
                    _weapon.ProjectilePool.AddObjectToPool(this);
                }
            }
        }

        public void InitializeProjectile(PlayerWeapon weapon, ProjectileData projectileData)
        {
            _initialized = true;
            _weapon = weapon;
            _projectileData = projectileData;

            _numberOfEnemiesPassedThrough = 0;

            if (_initialScale == Vector3.zero)
            {
                _initialScale = transform.localScale;
            }

            transform.localScale = _initialScale;
            transform.localScale *= projectileData.ProjectileSizeMultiplier;

            if (_projectileData.WeaponDuration > 0)
            {
                StartCoroutine(DisableAfterProjectileDuration());
            }
        }

        public void InitializeProjectile(PlayerWeapon weapon, ProjectileData projectileData, Vector3 direction)
        {
            direction.Normalize();
            _direction = direction;

            InitializeProjectile(weapon, projectileData);
        }

        protected virtual void MoveProjectile()
        {
            transform.position = transform.position + (_direction * Time.deltaTime * _projectileData.ProjectileSpeed);
        }

        protected virtual void OnTriggerEnter2D(Collider2D other) 
        { 
            if (other.gameObject)
            {
                Enemy enemy = other.gameObject.GetComponent<Enemy>();
                if (enemy)
                {
                    OnContactWithEnemy(enemy);

                    if (!_projectileData.CanPassThroughUnlimitedEnemies)
                    {
                        _numberOfEnemiesPassedThrough++;

                        if (_numberOfEnemiesPassedThrough > _projectileData.NumberOfEnemiesCanPassThrough)
                        {
                            _weapon.ProjectilePool.AddObjectToPool(this);
                        }
                    }
                    else
                    {
                        _enemiesInContactWith.Add(enemy);
                        enemy.HealthComponent.OnKilled -= RemoveEnemy;
                        enemy.HealthComponent.OnKilled += RemoveEnemy;

                        if (_enemiesInContactWith.Count == 1)
                        {
                            StartCoroutine(_damageAllEnemiesInRange);
                        }
                    }
                }
            }
        }

        protected virtual void OnTriggerExit2D(Collider2D other)
        {
            if (_projectileData.CanPassThroughUnlimitedEnemies)
            {
                Enemy enemy = other.gameObject.GetComponent<Enemy>();
                if (enemy)
                {
                    RemoveEnemy(enemy);
                }
            }
        }

        protected virtual void OnContactWithEnemy(Enemy enemy)
        {
            OnContactWithEnemy(enemy, _projectileData.DamageToDeal);
        }

        protected virtual void OnContactWithEnemy(Enemy enemy, float damageToDo)
        {
            if (enemy)
            {
                if (_projectileData.CanKnockback && _projectileData.KnockbackAmount > 0)
                {
                    Vector2 knockbackDirection = enemy.transform.position - _weapon.OwningPlayer.transform.position;
                    knockbackDirection.Normalize();
                    knockbackDirection *= _projectileData.KnockbackAmount;
                    enemy.CharacterMovementComponent.AddKnockback(knockbackDirection, _projectileData.KnockbackTime);
                }

                foreach (StatusEffectAttackData statusEffectAttack in _statusEffectsToApply)
                {
                    CharacterStatusEffectsManager statusEffectsManager = enemy.StatusEffectsManager;
                    if (statusEffectsManager)
                    {
                        statusEffectsManager.IncrementStacks(statusEffectAttack.EffectType, statusEffectAttack.StacksToAdd);
                    }
                }

                EnemyHealthComponent healthComponent = enemy.GetComponent<EnemyHealthComponent>();
                if (healthComponent && damageToDo > 0)
                {
                    bool killsEnemy = healthComponent.DoesDamageKill(damageToDo);
                    healthComponent.DoDamage(damageToDo);

                    if (killsEnemy)
                    {
                        AddXP(enemy);
                    }
                }
            }
        }

        private void AddXP(Enemy enemy)
        {
            if (enemy && _weapon)
            {
                EnemyXPComponent enemyXP = enemy.GetComponent<EnemyXPComponent>();
                WeaponXPComponent weaponXP = _weapon.GetComponent<WeaponXPComponent>();
                if (enemyXP && weaponXP)
                {
                    weaponXP.AddXP(_weapon.OwningPlayer.UpgradeAttributesComponent.GetModifiedAttributeValue(UpgradeAttribute.XPMultiplier, enemyXP.XpToGiveOnKill));
                }
            }
        }

        private void RemoveEnemy(Character deadCharacter)
        {
            Enemy enemy = deadCharacter as Enemy;
            if (enemy && _enemiesInContactWith.Contains(enemy))
            {
                enemy.HealthComponent.OnKilled -= RemoveEnemy;
                _enemiesInContactWith.Remove(enemy);

                if (_enemiesInContactWith.Count == 0)
                {
                    StopCoroutine(_damageAllEnemiesInRange);
                }
            }
        }

        private IEnumerator DamageAllEnemiesInRange()
        {
            yield return _waitForTimeBetweenDamage;

            List<Enemy> enemiesToDamage = new List<Enemy>(_enemiesInContactWith);
            foreach (Enemy enemy in enemiesToDamage)
            {
                if (enemy)
                {
                    OnContactWithEnemy(enemy);
                }
            }
        }

        private IEnumerator DisableAfterProjectileDuration()
        {
            yield return new WaitForSeconds(_projectileData.WeaponDuration);

            _weapon.ProjectilePool.AddObjectToPool(this);
        }

        public virtual void ActivateObject(bool shouldActivate)
        {
            _boxCollider.enabled = shouldActivate;
            _spriteRenderer.enabled = shouldActivate;
            gameObject.SetActive(shouldActivate);
            
            if (!shouldActivate)
            {
                _initialized = false;
                StopAllCoroutines();
            }
        }
    }
}
