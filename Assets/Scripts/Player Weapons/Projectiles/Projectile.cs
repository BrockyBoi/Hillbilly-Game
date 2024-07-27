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
        public float ProjectileSpeed;
        public float ProjectileSizeMultiplier;
        public float DamageToDeal;
        public float TimeBetweenDamage;
        public float WeaponDuration;
        public int NumberOfEnemiesCanPassThrough;
        public bool CanPassThroughUnlimitedEnemies;

        public ProjectileData(float speed, float sizeMultiplier, float damageToDeal, float timeBetweenDamage, float weaponDuration, int numberOfEnemiesToPassThrough, bool canPassThroughUnlimitedEnemies)
        {
            this.ProjectileSpeed = speed;
            this.ProjectileSizeMultiplier = sizeMultiplier;
            this.DamageToDeal = damageToDeal;
            this.WeaponDuration = weaponDuration;
            this.TimeBetweenDamage = timeBetweenDamage;
            this.NumberOfEnemiesCanPassThrough = numberOfEnemiesToPassThrough;
            this.CanPassThroughUnlimitedEnemies = canPassThroughUnlimitedEnemies;
        }
    }

    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class Projectile : MonoBehaviour, IPoolableObject
    {
        protected ProjectileData _projectileData;
        protected PlayerWeapon _weapon;
        
        private int _numberOfEnemiesPassedThrough = 0;
        protected Vector3 _direction = Vector3.zero;
        WaitForSeconds _waitForTimeBetweenDamage;
        List<Enemy> _enemiesInContactWith = new List<Enemy>();

        private SpriteRenderer _spriteRenderer;
        private BoxCollider2D _boxCollider;

        [SerializeField]
        private float _maxDistanceFromPlayerBeforeDestroy = 15;

        IEnumerator _damageAllEnemiesInRange;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _boxCollider = GetComponent<BoxCollider2D>();
        }

        private void Start()
        {
            _damageAllEnemiesInRange = DamageAllEnemiesInRange();
        }

        private void Update()
        {
            MoveProjectile();

            float distanceFromPlayer = (transform.position - MainPlayer.Instance.transform.position).sqrMagnitude;
            if (distanceFromPlayer > _maxDistanceFromPlayerBeforeDestroy)
            {
                PoolableObjectsManager.Instance.AddObjectToPool(this, ObjectPoolTypes.Projectile);
            }
        }

        public void InitializeProjectile(PlayerWeapon weapon, ProjectileData projectileData)
        {
            _weapon = weapon;
            _projectileData = projectileData;
            transform.localScale *= projectileData.ProjectileSizeMultiplier;
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

        void OnTriggerEnter2D(Collider2D other) 
        { 
            if (other.gameObject)
            {
                Enemy enemy = other.gameObject.GetComponent<Enemy>();
                if (enemy)
                {
                    EnemyHealthComponent healthComponent = enemy.GetComponent<EnemyHealthComponent>();
                    if (healthComponent)
                    {
                        bool killsEnemy = healthComponent.DoesDamageKill(_projectileData.DamageToDeal);
                        healthComponent.DoDamage(_projectileData.DamageToDeal);

                        if (killsEnemy)
                        {
                            AddXP(enemy);
                        }
                    }

                    if (!_projectileData.CanPassThroughUnlimitedEnemies)
                    {
                        _numberOfEnemiesPassedThrough++;

                        if (_numberOfEnemiesPassedThrough >= _projectileData.NumberOfEnemiesCanPassThrough)
                        {
                            Destroy(gameObject);
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

        private void AddXP(Enemy enemy)
        {
            if (enemy && _weapon)
            {
                EnemyXPComponent enemyXP = enemy.GetComponent<EnemyXPComponent>();
                WeaponXPComponent weaponXP = _weapon.GetComponent<WeaponXPComponent>();
                if (enemyXP && weaponXP)
                {
                    weaponXP.AddXP(enemyXP.XpToGiveOnKill);
                }
            }
        }

        void OnTriggerExit2D(Collider2D other) 
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

        IEnumerator DamageAllEnemiesInRange()
        {
            yield return _waitForTimeBetweenDamage;

            foreach (Enemy enemy in _enemiesInContactWith)
            {
                if (enemy)
                {
                    EnemyHealthComponent healthComponent = enemy?.GetComponent<EnemyHealthComponent>();
                    healthComponent?.DoDamage(_projectileData.DamageToDeal);
                }
            }
        }

        public void ActivateObject(bool shouldActivate)
        {
            _boxCollider.enabled = shouldActivate;
            _spriteRenderer.enabled = shouldActivate;
            gameObject.SetActive(true);
        }
    }
}
