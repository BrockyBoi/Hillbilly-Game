using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XP
{
    public class EnemyXPComponent : MonoBehaviour
    {
        [SerializeField]
        private float _xpToGiveOnKill;
        public float XpToGiveOnKill { get { return _xpToGiveOnKill; } }
    }
}
