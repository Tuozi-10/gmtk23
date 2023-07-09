using System;
using System.Collections.Generic;
using UnityEngine;

namespace IAs
{
    public class DetectionManager : MonoBehaviour
    {
        private HashSet<AI> m_inRadius = new HashSet<AI>();
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<AI>() is not null)
            {
                m_inRadius.Add(other.GetComponent<AI>());
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.GetComponent<AI>() is not null)
            {
                m_inRadius.Remove(other.GetComponent<AI>());
            }
        }

        public bool TryGetFromType(AI.Team desiredTeam, out AI returnAI, AI from)
        {
            returnAI = null;
            if (m_inRadius.Count == 0)
            {
                return false;
            }
            
            foreach (var ai in m_inRadius)
            {
                if (ai.team != desiredTeam || from == ai)
                {
                    continue;
                }
                
                if (returnAI == null)
                {
                    returnAI = ai;
                    continue;
                }

                if (ai == null)
                {
                    continue;
                }
                
                if (Vector3.Distance(transform.position, ai.transform.position) <
                    Vector3.Distance(transform.position, returnAI.transform.position))
                {
                    returnAI = ai;
                }
            }

            return returnAI != null;
        }
    }
}