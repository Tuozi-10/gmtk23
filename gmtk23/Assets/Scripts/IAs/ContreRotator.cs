using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace IAs
{
    [RequireComponent(typeof(SortingGroup))]
    public class ContreRotator : MonoBehaviour
    {
        private SortingGroup m_sortingGroup;
        
        private void Awake()
        {
            m_sortingGroup = GetComponent<SortingGroup>();
        }

        private void Update()
        {
            m_sortingGroup.sortingOrder = -(int)(transform.position.z*10);
            transform.eulerAngles = Vector3.zero;
        }
    }
}