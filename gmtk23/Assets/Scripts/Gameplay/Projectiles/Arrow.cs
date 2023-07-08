using System;
using IAs;
using UnityEngine;

namespace Gameplay.Projectiles
{
    public class Arrow : MonoBehaviour
    {
        [SerializeField] private Transform childPivot;
        [SerializeField] private float speed;

        // Update is called once per frame
        void Update()
        {
            transform.position += m_direction * Time.deltaTime * speed;
            childPivot.transform.localScale = new Vector3(m_direction.x < 0 ? 1 : -1,1,1);
        }

        private Vector3 m_direction;
        private AI.Team m_team;
        private int m_damages;
        
        public void SetUp(Vector3 direction, AI.Team team, int damages)
        {
            m_direction = direction.normalized;
            m_team = team;
            m_damages = damages;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<AI>() == null)
            {
                return;
            }
            
            var AI = other.GetComponent<AI>();
            if (AI.team == m_team)
            {
                return;
            }
                
            AI.Hit(m_damages);
            transform.SetParent(other.transform);
            Destroy(this);
        }
    }
}
