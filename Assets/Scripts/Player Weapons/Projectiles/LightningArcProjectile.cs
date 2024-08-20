using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Weaponry;

namespace Weaponry
{
    public class LightningArcProjectile : BaseBeamProjectile
    {
        private bool _alreadyAttackedEnemies = false;

        protected override void AttackEnemies(List<Enemy> enemiesToAttack)
        {
            if (_enemiesToAttack.Count == 0)
            {
                return;
            }

            Vector3 startPos = MainPlayer.Instance.transform.position;

            _lineRenderer.positionCount = _enemiesToAttack.Count + 1;
            _lineRenderer.startColor = _beamColor;
            _lineRenderer.endColor = _beamColor;

            transform.position = startPos;

            _lineRenderer.SetPosition(0, startPos);
            for (int i = 0; i < _enemiesToAttack.Count; i++)
            {
                Enemy enemy = _enemiesToAttack[i];
                if (enemy)
                {
                    Vector3 endPos = enemy.transform.position;
                    float width = _defaultBeamWidth * _projectileData.ProjectileSizeMultiplier;
                    Vector2 sizeVector = new Vector2(width, width);

                    if (!_alreadyAttackedEnemies)
                    {
                        OnContactWithEnemy(enemy);
                        _alreadyAttackedEnemies = true;
                    }

                    _lineRenderer.SetPosition(i + 1, endPos);
                    _lineRenderer.startWidth = sizeVector.x * _visualWidthModifier;
                    _lineRenderer.endWidth = sizeVector.x * _visualWidthModifier;
                }
            }
        }

        public override void ActivateObject(bool shouldActivate)
        {
           base.ActivateObject(shouldActivate);

            if (!shouldActivate)
            {
                _alreadyAttackedEnemies = false;
            }
        }
    }
}
