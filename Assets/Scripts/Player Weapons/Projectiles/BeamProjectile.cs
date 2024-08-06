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
        private float _visualWidthModifier = .75f;

        [SerializeField]
        private Color _beamColor = Color.red;

        private LineRenderer _lineRenderer;
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
            RayCastToEnemies(_enemiesToAttack);
        }

        public override void ActivateObject(bool shouldActivate)
        {
            gameObject.SetActive(shouldActivate);

            if (!shouldActivate)
            {
                _initialized = false;
                StopAllCoroutines();
            }

            _lineRenderer.enabled = shouldActivate;
            _lineRenderer.positionCount = 0;
            _enemiesToAttack.Clear();
        }

        private void RayCastToEnemies(List<Enemy> enemiesToAttack)
        {
            if (enemiesToAttack.Count == 0)
            {
                return;
            }

            Vector3 startPos = MainPlayer.Instance.transform.position;

            _lineRenderer.positionCount = enemiesToAttack.Count + 1;
            _lineRenderer.startColor = _beamColor;

            transform.position = startPos;

            _lineRenderer.SetPosition(0, startPos);
            List<Enemy> enemiesToIgnoreFurtherDamage = new List<Enemy>();
            for (int i = 0; i < enemiesToAttack.Count; i++)
            {
                Enemy enemy = enemiesToAttack[i];
                if (enemy)
                {
                    Vector3 endPos = enemy.transform.position;
                    Vector3 dir = endPos - startPos;
                    float width = _defaultBeamWidth * _projectileData.ProjectileSizeMultiplier;
                    Vector2 sizeVector = new Vector2(width, width);

                    float angle = Vector3.Angle(i == 0 ? MainPlayer.Instance.transform.up : enemy.transform.up, dir);
                    ExtDebug.DrawBoxCastBox(startPos, sizeVector * .5f, Quaternion.Euler(new Vector3(0, angle, 0)), dir, dir.magnitude, Color.green);
                    RaycastHit2D[] objectsInLine = Physics2D.BoxCastAll(startPos, sizeVector, angle, dir, dir.magnitude, LayerMask.GetMask("Enemy"));
                    Debug.Log(objectsInLine.Length);
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
                    _lineRenderer.startWidth = sizeVector.x * _visualWidthModifier;
                    _lineRenderer.endWidth = sizeVector.x * _visualWidthModifier;
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
                _enemiesToAttack.AddRange(extraEnemies.Where(x => !_enemiesToAttack.Contains(x)));
            }
        }
    }











    public static class ExtDebug
    {
        //Draws just the box at where it is currently hitting.
        public static void DrawBoxCastOnHit(Vector3 origin, Vector3 halfExtents, Quaternion orientation, Vector3 direction, float hitInfoDistance, Color color)
        {
            origin = CastCenterOnCollision(origin, direction, hitInfoDistance);
            DrawBox(origin, halfExtents, orientation, color);
        }

        //Draws the full box from start of cast to its end distance. Can also pass in hitInfoDistance instead of full distance
        public static void DrawBoxCastBox(Vector3 origin, Vector3 halfExtents, Quaternion orientation, Vector3 direction, float distance, Color color)
        {
            direction.Normalize();
            Box bottomBox = new Box(origin, halfExtents, orientation);
            Box topBox = new Box(origin + (direction * distance), halfExtents, orientation);

            Debug.DrawLine(bottomBox.backBottomLeft, topBox.backBottomLeft, color);
            Debug.DrawLine(bottomBox.backBottomRight, topBox.backBottomRight, color);
            Debug.DrawLine(bottomBox.backTopLeft, topBox.backTopLeft, color);
            Debug.DrawLine(bottomBox.backTopRight, topBox.backTopRight, color);
            Debug.DrawLine(bottomBox.frontTopLeft, topBox.frontTopLeft, color);
            Debug.DrawLine(bottomBox.frontTopRight, topBox.frontTopRight, color);
            Debug.DrawLine(bottomBox.frontBottomLeft, topBox.frontBottomLeft, color);
            Debug.DrawLine(bottomBox.frontBottomRight, topBox.frontBottomRight, color);

            DrawBox(bottomBox, color);
            DrawBox(topBox, color);
        }

        public static void DrawBox(Vector3 origin, Vector3 halfExtents, Quaternion orientation, Color color)
        {
            DrawBox(new Box(origin, halfExtents, orientation), color);
        }
        public static void DrawBox(Box box, Color color)
        {
            Debug.DrawLine(box.frontTopLeft, box.frontTopRight, color);
            Debug.DrawLine(box.frontTopRight, box.frontBottomRight, color);
            Debug.DrawLine(box.frontBottomRight, box.frontBottomLeft, color);
            Debug.DrawLine(box.frontBottomLeft, box.frontTopLeft, color);

            Debug.DrawLine(box.backTopLeft, box.backTopRight, color);
            Debug.DrawLine(box.backTopRight, box.backBottomRight, color);
            Debug.DrawLine(box.backBottomRight, box.backBottomLeft, color);
            Debug.DrawLine(box.backBottomLeft, box.backTopLeft, color);

            Debug.DrawLine(box.frontTopLeft, box.backTopLeft, color);
            Debug.DrawLine(box.frontTopRight, box.backTopRight, color);
            Debug.DrawLine(box.frontBottomRight, box.backBottomRight, color);
            Debug.DrawLine(box.frontBottomLeft, box.backBottomLeft, color);
        }

        public struct Box
        {
            public Vector3 localFrontTopLeft { get; private set; }
            public Vector3 localFrontTopRight { get; private set; }
            public Vector3 localFrontBottomLeft { get; private set; }
            public Vector3 localFrontBottomRight { get; private set; }
            public Vector3 localBackTopLeft { get { return -localFrontBottomRight; } }
            public Vector3 localBackTopRight { get { return -localFrontBottomLeft; } }
            public Vector3 localBackBottomLeft { get { return -localFrontTopRight; } }
            public Vector3 localBackBottomRight { get { return -localFrontTopLeft; } }

            public Vector3 frontTopLeft { get { return localFrontTopLeft + origin; } }
            public Vector3 frontTopRight { get { return localFrontTopRight + origin; } }
            public Vector3 frontBottomLeft { get { return localFrontBottomLeft + origin; } }
            public Vector3 frontBottomRight { get { return localFrontBottomRight + origin; } }
            public Vector3 backTopLeft { get { return localBackTopLeft + origin; } }
            public Vector3 backTopRight { get { return localBackTopRight + origin; } }
            public Vector3 backBottomLeft { get { return localBackBottomLeft + origin; } }
            public Vector3 backBottomRight { get { return localBackBottomRight + origin; } }

            public Vector3 origin { get; private set; }

            public Box(Vector3 origin, Vector3 halfExtents, Quaternion orientation) : this(origin, halfExtents)
            {
                Rotate(orientation);
            }
            public Box(Vector3 origin, Vector3 halfExtents)
            {
                this.localFrontTopLeft = new Vector3(-halfExtents.x, halfExtents.y, -halfExtents.z);
                this.localFrontTopRight = new Vector3(halfExtents.x, halfExtents.y, -halfExtents.z);
                this.localFrontBottomLeft = new Vector3(-halfExtents.x, -halfExtents.y, -halfExtents.z);
                this.localFrontBottomRight = new Vector3(halfExtents.x, -halfExtents.y, -halfExtents.z);

                this.origin = origin;
            }


            public void Rotate(Quaternion orientation)
            {
                localFrontTopLeft = RotatePointAroundPivot(localFrontTopLeft, Vector3.zero, orientation);
                localFrontTopRight = RotatePointAroundPivot(localFrontTopRight, Vector3.zero, orientation);
                localFrontBottomLeft = RotatePointAroundPivot(localFrontBottomLeft, Vector3.zero, orientation);
                localFrontBottomRight = RotatePointAroundPivot(localFrontBottomRight, Vector3.zero, orientation);
            }
        }

        //This should work for all cast types
        static Vector3 CastCenterOnCollision(Vector3 origin, Vector3 direction, float hitInfoDistance)
        {
            return origin + (direction.normalized * hitInfoDistance);
        }

        static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Quaternion rotation)
        {
            Vector3 direction = point - pivot;
            return pivot + rotation * direction;
        }
    }
}
