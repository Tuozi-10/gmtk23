using System;
using System.Collections.Generic;
using Gameplay;
using UnityEngine;
using UnityEngine.Serialization;

namespace IAs
{
    public class Pack : MonoBehaviour
    {
        public packTracking tracking;
        [SerializeField] private bool heroPack;
         public List<AI> m_ais;
        [SerializeField] public PackMobManager packMobManagerLink;
        
        private void Awake()
        {
            foreach (var ai in transform.GetComponentsInChildren<AI>())
            {
                m_ais.Add(ai);
                ai.currentPack = this;
            }
        }
    }
}