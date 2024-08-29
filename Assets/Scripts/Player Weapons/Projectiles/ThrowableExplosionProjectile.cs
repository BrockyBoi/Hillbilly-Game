using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Weaponry
{
    public class ThrowableExplosionProjectile : BaseProjectile
    {
        Vector3 _startingPosition = Vector3.zero;
        Vector3 _finalDestination = Vector3.zero;

        float _timeMoved = 0;

        [Title("Throwable Stats")]
        [SerializeField]
        float _timeToMove = 1;

        [SerializeField]
        float _timeUntilExplosion = 1.5f;

        public override void InitializeProjectile(PlayerWeapon weapon, ProjectileData projectileData, Vector3 finalDestination)
        {
            base.InitializeProjectile(weapon, projectileData, finalDestination);

            _startingPosition = weapon.transform.position;
            _finalDestination = finalDestination;
            _timeMoved = 0;

            _boxCollider.enabled = false;

            StartCoroutine(WaitUntilExplode());

            if (!projectileData.CanProjectileExplode)
            {
                Debug.LogError("Throwable projectiles must be able to explode");
            }
        }

        protected override void MoveProjectile()
        {
            _timeMoved += Time.deltaTime;
            if (_timeMoved < _timeToMove)
            {
                transform.position = Vector3.Lerp(_startingPosition, _finalDestination, _timeMoved / _timeToMove);
            }
        }

        private IEnumerator WaitUntilExplode()
        {
            yield return new WaitForSeconds(_timeUntilExplosion + _timeToMove);

            BaseProjectile explosion = _poolForExtraProjectileToSpawnOnContact.GetPoolableObject();
            if (explosion)
            {
                explosion.InitializeProjectile(_weapon, _projectileData);
            }

            _weapon.ProjectilePool.AddObjectToPool(this);
        }

        public override void ActivateObject(bool shouldActivate)
        {
            base.ActivateObject(shouldActivate);

            // Collider should never be activated
            _boxCollider.enabled = false;
        }
    }
}
