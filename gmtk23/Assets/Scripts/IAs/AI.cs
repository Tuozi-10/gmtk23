using System;
using System.Collections.Generic;
using Items;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace IAs
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class AI : MonoBehaviour
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

        [SerializeField]
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

        #region Job

        public enum Jobs
        {
            Support,
            Cac,
            Shooter
        }

        protected Jobs m_currentJob;
        
        #endregion

        [SerializeField] private float m_magnitudeWander = 3;
        
        protected NavMeshAgent agent;

        [SerializeField] private Animator m_animator;

        private Vector3 m_positionInit;
        
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
            RefreshStuffs();
            m_positionInit = transform.position;
        }
        
        private void Update()
        {
            UpdateStateMachine();
        }

        #region Items

        [SerializeField] protected Weapon m_weapon;
        [SerializeField] protected Armor m_armor;

        [SerializeField] private SpriteRenderer m_armorSlot;
        [SerializeField] private SpriteRenderer m_weaponSlot;

        public void SetWeapon(Weapon weapon)
        {
            m_weapon = weapon;
            m_weaponSlot.sprite = weapon.sprite;
        }
        
        public void SetArmor(Armor armor)
        {
            m_armor = armor;
            m_armorSlot.sprite = armor.sprite;
        }

        public void RefreshStuffs()
        {
            m_armorSlot.sprite = m_armor is null ? null : m_armor.sprite;
            m_weaponSlot.sprite = m_weapon is null ? null : m_weapon.sprite;
        }
        
        #endregion

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

        protected virtual void DoIdle()
        { 
            m_animator.Play("Idle");
            
        }

        protected virtual void DoWander()
        {
            m_animator.Play("Move");   
            if (agent.remainingDistance < 0.5f)
            {
                agent.destination = new Vector3(m_positionInit.x + Random.Range(-m_magnitudeWander, m_magnitudeWander),
                    m_positionInit.y, m_positionInit.z + Random.Range(-m_magnitudeWander, m_magnitudeWander)); 
            }
        }
        protected virtual void DoHunting(){ }
        protected virtual void DoAttacking(){ }
        protected virtual void DoDead(){ }

        #endregion
        
    }
}
