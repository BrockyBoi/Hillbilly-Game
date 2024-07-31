using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Weaponry
{
    public class BeamProjectile : BaseProjectile
    {
        private Enemy _enemyToAttack;
        [SerializeField]
        private Color _beamColor = Color.red;

        Mesh _beamMesh;
        protected override void MoveProjectile()
        {
            if (_enemyToAttack)
            {
                Vector3 startPos = MainPlayer.Instance.transform.position;
                Vector3 endPos = _enemyToAttack.transform.position;
                Vector3 dir = endPos - startPos;
                Debug.DrawLine(startPos, endPos);

                RaycastHit2D[] objectsInLine = Physics2D.RaycastAll(startPos, dir, dir.magnitude, LayerMask.NameToLayer("Projectile"));
                for (int i = 0; i < objectsInLine.Length; i++)
                {
                    Enemy enemy = objectsInLine[i].collider.gameObject.GetComponent<Enemy>();
                    if (enemy)
                    {
                        EnemyHealthComponent enemyHealth = enemy.HealthComponent as EnemyHealthComponent;
                        if (enemyHealth)
                        {
                            enemyHealth.DoDamage(_projectileData.DamageToDeal * Time.deltaTime);
                        }
                    }
                }
            }
        }

        public void InitializeProjectile(PlayerWeapon weapon, ProjectileData projectileData, Enemy enemyToAttack)
        {
            InitializeProjectile(weapon, projectileData);
            _enemyToAttack = enemyToAttack;
        }
    }
}
