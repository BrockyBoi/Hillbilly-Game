using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XP
{
    public class XPPoolManager : MonoBehaviour
    {
        public static XPPoolManager Instance { get; private set; }

        [SerializeField]
        private GameObject _xpOrbPrefab;

        private PoolableObjectsComponent<XPOrb> _xpOrbPool = new PoolableObjectsComponent<XPOrb>();
        public PoolableObjectsComponent<XPOrb> XPOrbPool { get { return _xpOrbPool;} }

        private void Awake()
        {
            Instance = this;
        }

        void Start ()
        {
            _xpOrbPool.Initialize(_xpOrbPrefab);
        }
       
    }
}
