using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Weaponry
{
    [RequireComponent(typeof(LineRenderer))]
    public class BeamProjectile : BaseProjectile
    {
        private List<Enemy> _enemiesToAttack = new List<Enemy>();

        [SerializeField]
        private float _defaultBeamWidth = 1f;

        [SerializeField]
        private Color _beamColor = Color.red;

        LineRenderer _lineRenderer;
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

            _enemiesToAttack = _enemiesToAttack.Where(enemy => enemy.IsAlive()).ToList();
            RayCastToEnemies(_enemiesToAttack);
        }

        public override void ActivateObject(bool shouldActivate)
        {
            gameObject.SetActive(true);

            if (!shouldActivate)
            {
                _initialized = false;
                StopAllCoroutines();
            }

            _lineRenderer.enabled = shouldActivate;
        }

        private void RayCastToEnemies(List<Enemy> enemiesToAttack)
        {
            Vector3 startPos = MainPlayer.Instance.transform.position;

            _lineRenderer.positionCount = enemiesToAttack.Count + 1;
            _lineRenderer.startColor = _beamColor;

            transform.position = startPos;

            _lineRenderer.SetPosition(0, startPos);
            Debug.Log("Attacking " + enemiesToAttack.Count + " enemies");
            List<Enemy> enemiesToIgnoreFurtherDamage = new List<Enemy>();
            for (int i = 0; i < enemiesToAttack.Count; i++)
            {
                Enemy enemy = enemiesToAttack[i];
                if (enemy)
                {
                    Vector3 endPos = enemy.transform.position;
                    Vector3 dir = endPos - startPos;
                    Vector2 sizeVector = new Vector2(_defaultBeamWidth * _projectileData.ProjectileSizeMultiplier, dir.magnitude);

                    float angle = Vector3.Angle(i == 0 ? MainPlayer.Instance.transform.up : enemy.transform.up, dir);
                    RaycastHit2D[] objectsInLine = Physics2D.BoxCastAll(startPos, sizeVector, angle, dir, dir.magnitude, LayerMask.GetMask("Enemy"));
                    for (int j = 0; j < objectsInLine.Length; j++)
                    {
                        Enemy enemyInCollision = objectsInLine[j].collider.gameObject.GetComponent<Enemy>();
                        if (enemyInCollision)
                        {
                            if (enemiesToIgnoreFurtherDamage.Contains(enemyInCollision))
                            {
                                continue;
                            }

                            EnemyHealthComponent enemyHealth = enemyInCollision.HealthComponent as EnemyHealthComponent;
                            if (enemyHealth)
                            {
                                enemyHealth.DoDamage(_projectileData.DamageToDeal * Time.deltaTime);
                                enemiesToIgnoreFurtherDamage.Add(enemyInCollision);
                            }
                        }
                    }

                    startPos = enemy.transform.position;

                    _lineRenderer.SetPosition(i + 1, endPos);
                    _lineRenderer.startWidth = sizeVector.x * .075f;
                    _lineRenderer.endWidth = sizeVector.x * .075f;
                }
            }
        }

        private Enemy GetRandomEnemy()
        {
            Enemy enemy = null;
            FireAtRandomEnemyWeapon randomWeapon = _weapon as FireAtRandomEnemyWeapon;
            if (randomWeapon)
            {
                enemy = randomWeapon.GetRandomEnemy();
            }

            return enemy;
        }

        private void SetNewEnemiesToAttack()
        {
            _enemiesToAttack.Clear();
            _enemiesToAttack.Add(GetRandomEnemy());
            if (_projectileData.ProjectileArcCount > 0 && _enemiesToAttack[0])
            {
                List<Enemy> extraEnemies = EnemySpawnerController.Instance.GetRandomEnemiesWithinRange(_projectileData.ProjectileArcCount, _enemiesToAttack[0].transform.position, 10f);

                List<Enemy> bla = extraEnemies.Where(x => !_enemiesToAttack.Contains(x)).ToList();
                _enemiesToAttack.AddRange(extraEnemies.Where(x => !_enemiesToAttack.Contains(x)));
            }
        }
    }
}
