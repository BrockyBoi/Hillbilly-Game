using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XP
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(CircleCollider2D))]
    public class XPOrb : MonoBehaviour, IPoolableObject
    {
        [SerializeField]
        private float _timeToMoveTowardsPlayer = 1f;

        [SerializeField]
        private float _minDistToAutoCollect = 5;

        private int _xpToAdd = 0;

        CircleCollider2D _collider;
        SpriteRenderer _spriteRenderer;

        private bool _movingTowardsPlayer = false;

        void Awake()
        {
            _collider = GetComponent<CircleCollider2D>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void InitializeXPOrb(int xpToAdd)
        {
            _xpToAdd = xpToAdd;
        }

        public void OnEnterPlayerPickupRange()
        {
            if (!_movingTowardsPlayer)
            {
                StartCoroutine(MoveTowardsPlayer());
            }
        }

        IEnumerator MoveTowardsPlayer()
        {
            _movingTowardsPlayer = true;

            MainPlayer player = MainPlayer.Instance;
            Vector3 startingOrbPosition = transform.position;
            float timeMoved = 0f;

            while (timeMoved < _timeToMoveTowardsPlayer)
            {
                timeMoved += Time.deltaTime;
                transform.position = Vector3.Lerp(startingOrbPosition, player.transform.position, timeMoved / _timeToMoveTowardsPlayer);

                Vector3 distToPlayer = player.transform.position - transform.position;
                if (distToPlayer.sqrMagnitude < _minDistToAutoCollect)
                {
                    break;
                }
                yield return null;
            }

            player.PlayerXPComponent.AddXP(_xpToAdd);
            XPPoolManager.Instance.XPOrbPool.AddObjectToPool(this);

            _movingTowardsPlayer = false;
        }

        public void ActivateObject(bool shouldActivate)
        {
            gameObject.SetActive(shouldActivate);
            _spriteRenderer.enabled = shouldActivate;
            _collider.enabled = shouldActivate;

            if (!shouldActivate)
            {
                StopAllCoroutines();
            }
        }
    }
}
