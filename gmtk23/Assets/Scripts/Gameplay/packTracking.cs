using System;
using IAs;
using UnityEngine;

namespace Gameplay
{
    public class packTracking : MonoBehaviour
    {
        public Road m_currentRoad;
        public int m_currentRoadId;

        private const float speedPack = 3f;

        private void Start()
        {
            m_currentRoad = RoadManager.GetRandomRoad();
        }

        public void Update()
        {
            if (countMobs > 0)
            {
                return; // clean area first
            }
            
            CalculateDirection();

            transform.position += m_currentDirection * speedPack * Time.deltaTime;
        }

        private Vector3 m_currentDirection;

        private void CalculateDirection()
        {
            if (m_currentRoadId < m_currentRoad.waypoints.Count - 1)
            {
                m_currentDirection = (m_currentRoad.waypoints[m_currentRoadId].position - transform.position).normalized;
            }
            else
            {
                // reached boss
            }

            if (Vector3.Distance(m_currentRoad.waypoints[m_currentRoadId].position, transform.position) < 1.5f)
            {
                m_currentRoadId++;
            }
        }

        private int countMobs;
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<AI>() != null)
            {
                var ai = other.GetComponent<AI>();
                if (ai.team == AI.Team.Orc)
                {
                    countMobs++;
                }
            }
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (other.GetComponent<AI>() != null)
            {
                var ai = other.GetComponent<AI>();
                if (ai.team == AI.Team.Orc)
                {
                    countMobs--;
                }
            }
        }
        
    }
}
