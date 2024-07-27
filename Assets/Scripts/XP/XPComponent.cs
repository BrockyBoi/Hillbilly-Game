using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XP
{
    public class XPComponent : MonoBehaviour
    {
        protected int _currentLevel = 0;
        public int CurrentLevel { get { return _currentLevel; } }

        protected float _currentXP = 0;
        public float CurrentXP { get { return _currentXP; } }

        [SerializeField]
        List<float> _xpNeededPerLevel;

        public virtual void AddXP(float xp)
        {
            Debug.Log("Adding " + xp + "xp");
            _currentXP += xp;

            float xpNeeded = GetXPNeeded();
            while (_currentXP > xpNeeded && xpNeeded > 0)
            {
                _currentXP -= xpNeeded;
                LevelUp();
            }
        }

        protected virtual void LevelUp()
        {
            _currentLevel = Mathf.Clamp(_currentLevel + 1, 0, _xpNeededPerLevel.Count - 1);
            Debug.Log("Leveld up to " + _currentLevel + " level");
        }

        public float GetXPNeeded()
        {
            if (_currentLevel < _xpNeededPerLevel.Count && _xpNeededPerLevel.Count > 0)
            {
                return _xpNeededPerLevel[_currentLevel];

            }

            return -1;
        }
    }
}
