using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XP
{
    public class EnemyXPComponent : MonoBehaviour
    {
        [SerializeField]
        private int _xpToGiveOnKill;
        public int XpToGiveOnKill { get { return _xpToGiveOnKill; } }
    }
}
