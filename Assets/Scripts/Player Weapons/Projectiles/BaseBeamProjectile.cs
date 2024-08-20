using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Weaponry
{
    [RequireComponent(typeof(LineRenderer))]
    public abstract class BaseBeamProjectile : BaseProjectile
    {
        protected List<Enemy> _enemiesToAttack = new List<Enemy>();

        [Title("Beam Data")]
        [SerializeField]
        protected float _defaultBeamWidth = 1f;

        [SerializeField]
        protected float _visualWidthModifier = .75f;

        [SerializeField]
        protected Color _beamColor = Color.red;

        protected LineRenderer _lineRenderer;

        protected override void Awake()
        {
            base.Awake();

            _lineRenderer = GetComponent<LineRenderer>();
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            _lineRenderer.positionCount = 0;
            _enemiesToAttack.Clear();
        }

        protected override void MoveProjectile()
        {
            Enemy firstEnemy = _enemiesToAttack.Count > 0 ? _enemiesToAttack[0] : null;
            if (!firstEnemy || !firstEnemy.IsAlive())
            {
                SetNewEnemiesToAttack();
            }

            _enemiesToAttack = _enemiesToAttack.Where(enemy => enemy && enemy.IsAlive()).ToList();
            AttackEnemies(_enemiesToAttack);
        }

        protected abstract void AttackEnemies(List<Enemy> enemiesToAttack);

        protected Enemy GetRandomEnemy()
        {
            Enemy enemy = null;
            FireAtRandomEnemyWeapon randomWeapon = _weapon as FireAtRandomEnemyWeapon;
            if (randomWeapon)
            {
                enemy = randomWeapon.GetRandomEnemy();
            }

            return enemy;
        }

        protected void SetNewEnemiesToAttack()
        {
            _enemiesToAttack.Clear();
            _enemiesToAttack.Add(GetRandomEnemy());
            if (_projectileData.ProjectileArcCount > 0 && _enemiesToAttack[0])
            {
                List<Enemy> extraEnemies = EnemySpawnerController.Instance.GetRandomEnemiesWithinRange(_projectileData.ProjectileArcCount, _enemiesToAttack[0].transform.position, 10f);
                _enemiesToAttack.AddRange(extraEnemies.Where(x => !_enemiesToAttack.Contains(x)));
            }
        }

        public override void ActivateObject(bool shouldActivate)
        {
            base.ActivateObject(shouldActivate);

            _lineRenderer.enabled = shouldActivate;
            _lineRenderer.positionCount = 0;
            _enemiesToAttack.Clear();
        }
    }
}
