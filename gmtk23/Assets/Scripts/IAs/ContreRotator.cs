﻿using System;
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
        private AI ai;
        
        private void Awake()
        {
            m_sortingGroup = GetComponent<SortingGroup>();
            m_agent = transform.parent.GetComponent<NavMeshAgent>();
            
            if(m_agent != null)
            ai = m_agent.GetComponent<AI>();
        }

        private void Update()
        {
            m_sortingGroup.sortingOrder = -(int)(transform.position.z*30);
            transform.eulerAngles = Vector3.zero;

            if (m_agent == null)
            {
                return;
            }
            
            if (m_agent.velocity.magnitude > 0.2f)
            {
                transform.localScale = new Vector3(m_agent.destination.x < transform.position.x ? -1 : 1, 1, 1);
            }
            else
            {
                if (ai != null && ai.targetAI != null)
                {
                    transform.localScale = new Vector3(ai.targetAI.transform.position.x < ai.transform.position.x ? -1 : 1, 1, 1);
                }
            }
        }
    }
}