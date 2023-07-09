using System;
using System.Collections.Generic;
using UnityEngine;

namespace IAs
{
    public class Pack : MonoBehaviour
    {
        [SerializeField] private bool heroPack;
        [SerializeField] private List<AI> m_ais;
        
        private void Awake()
        {
            foreach (var ai in transform.GetComponentsInChildren<AI>())
            {
                m_ais.Add(ai);
            }
        }
    }
}