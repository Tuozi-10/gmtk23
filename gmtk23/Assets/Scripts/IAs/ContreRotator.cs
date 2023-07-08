using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

namespace IAs
{
    [RequireComponent(typeof(SortingGroup))]
    public class ContreRotator : MonoBehaviour
    {
        private SortingGroup m_sortingGroup;
        private NavMeshAgent m_agent;
        
        private void Awake()
        {
            m_sortingGroup = GetComponent<SortingGroup>();
            m_agent = transform.parent.GetComponent<NavMeshAgent>();
        }

        private void Update()
        {
            m_sortingGroup.sortingOrder = -(int)(transform.position.z*10);
            transform.eulerAngles = Vector3.zero;

            if (m_agent.velocity.magnitude > 0.2f)
            {
                transform.localScale = new Vector3(m_agent.destination.x < transform.position.x ? -1 : 1, 1, 1);
            }
        }
    }
}