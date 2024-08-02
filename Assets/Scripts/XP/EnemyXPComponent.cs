using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XP
{
    public class EnemyXPComponent : MonoBehaviour
    {
        Enemy _owningEnemy;
        [SerializeField]
        private int _xpToGiveOnKill;
        public int XpToGiveOnKill { get { return _xpToGiveOnKill; } }

        PoolableObjectsComponent<XPOrb> _orbPool;

        private void Awake()
        {
            _owningEnemy = GetComponent<Enemy>();
        }

        private void OnEnable()
        {
            if (_owningEnemy)
            {
                EnemyHealthComponent healthComponent = _owningEnemy.HealthComponent as EnemyHealthComponent;
                if (healthComponent)
                {
                    healthComponent.OnKilled += SpawnOrbOnGround;
                }
            }
        }

        private void OnDisable()
        {
            if (_owningEnemy)
            {
                EnemyHealthComponent healthComponent = _owningEnemy.HealthComponent as EnemyHealthComponent;
                if (healthComponent)
                {
                    healthComponent.OnKilled -= SpawnOrbOnGround;
                }
            }
        }

        private void SpawnOrbOnGround(Character characterKilled)
        {
            XPOrb orb = _orbPool.GetPoolableObject();
            int xpToGive = Mathf.RoundToInt(_xpToGiveOnKill * EnemyDifficultyManager.Instance.GetBonusXP());
            orb.InitializeXPOrb(xpToGive);
        }
    }
}
