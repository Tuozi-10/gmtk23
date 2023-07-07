using System;
using UnityEngine;
using UnityEngine.AI;

namespace IAs
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class AbstractIA : MonoBehaviour
    {

        #region BodyTypes
        
        public enum BodyType
        {
            Fat,
            Medium,
            Small
        }

        private BodyType m_currentBodyType;
        
        #endregion
        
        #region states

        public enum States
        {
            Idle,
            Wander,
            Hunting,
            Attacking,
            Dead
        }

        private States m_currentState = States.Idle;

        #endregion
        
        #region Team
        
        public enum Team
        {
            Orc,
            Hero
        }

        private Team m_currentTeam = Team.Hero;

        public virtual void ChangeTeam(Team newTeam)
        {
            m_currentTeam = newTeam;
        }
        
        #endregion

        protected NavMeshAgent agent;
        
        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            Init();
        }

        /// <summary>
        /// to override to be sure you have data from awake
        /// </summary>
        protected virtual void Init()
        {
            
        }
        
        private void Update()
        {
            UpdateStateMachine();
        }

        #region Movement

        // movement on map ( flee, wander, .. )
        protected Vector3 m_targetPosition;
        
        // entity targeting ( can be ally or enemy for support or attack )
        protected Transform m_targetTransform;

        protected virtual void Move()
        {
            if (m_targetTransform != null)
            {
                agent.SetDestination(m_targetTransform.position);
                return;
            }
            
            agent.SetDestination(m_targetPosition);
        }
        
        #endregion
        
        #region State Machine
        
        private void UpdateStateMachine()
        {
            switch (m_currentState)
            {
                case States.Idle:       DoIdle(); break;
                case States.Wander:     DoWander(); break;
                case States.Hunting:    DoHunting(); break;
                case States.Attacking:  DoAttacking(); break;
                case States.Dead:       DoDead(); break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected virtual void DoIdle(){ }
        protected virtual void DoWander(){ }
        protected virtual void DoHunting(){ }
        protected virtual void DoAttacking(){ }
        protected virtual void DoDead(){ }

        #endregion
        
    }
}
