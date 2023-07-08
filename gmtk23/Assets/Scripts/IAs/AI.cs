using System;
using Items;
using Managers;
using UI;
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
            
        [SerializeField]
        private BodyType m_currentBodyType;
        
        #endregion
        
        #region states

        public enum States
        {
            Idle,
            Wander,
            Chasing,
            Attacking,
            Flee,
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
        [SerializeField]
        private Team m_currentTeam = Team.Hero;
        public Team team => m_currentTeam;
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

        [SerializeField]
        protected Jobs m_currentJob;
        
        #endregion

        #region Hp

        private int m_currentHp;
        [SerializeField] private int m_baseHp = 5;
        
        #endregion
        
        [Space,SerializeField] private float m_speedSmall = 3.5f;
        [SerializeField] private float m_speedMedium = 2.5f;
        [SerializeField] private float m_speedFat = 1.5f;
        
        [Space, SerializeField] private float m_magnitudeWander = 3;
        [Space, SerializeField] private float m_radiusAgro = 3;
        [SerializeField] private float m_distanceForget;
        [SerializeField] private SphereCollider m_triggerDetection;
        [SerializeField] private float m_distanceAttackCac = 1.5f;
        [SerializeField] private float m_distanceAttackDistance = 5.5f;
        [SerializeField] private float m_fleeTooNearAttackDistance = 3.5f;

        [SerializeField] private AiHp m_aiHp;

        [SerializeField] private float attackSpeedRate = 1f;
        
        private DetectionManager m_detectionManager;
        
        protected NavMeshAgent agent;

        [Space, SerializeField] private Animator m_animator;

        private Vector3 m_positionInit;
        
        private void Awake()
        {
            Init();
        }

        /// <summary>
        /// to override to be sure you have data from awake
        /// </summary>
        protected virtual void Init()
        {
            m_detectionManager = m_triggerDetection.GetComponent<DetectionManager>();
            agent = GetComponent<NavMeshAgent>();
            RefreshStuffs();
            m_positionInit = transform.position;
            agent.speed = m_currentBodyType == BodyType.Fat ? m_speedFat :
                m_currentBodyType == BodyType.Medium ? m_speedMedium : m_speedSmall;

            m_currentHp = m_baseHp;
            RefreshHp();
            
            m_triggerDetection.radius = m_radiusAgro;
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
            RefreshStuffs();
        }
        
        public void SetArmor(Armor armor)
        {
            m_armor = armor;
            RefreshStuffs();
        }

        public void RefreshStuffs()
        {
            m_armorSlot.sprite = m_armor == null ? null : m_armor.sprite;
            m_weaponSlot.sprite = m_weapon == null ? null : m_weapon.sprite;
            if (m_weapon != null)
            {
                m_currentJob = m_weapon.AssociatedJob;
                return;
            }

            m_currentJob = Jobs.Cac; // default, hit with hands ? or flee ?
        }
        
        #endregion

        #region Movement

        // movement on map ( flee, wander, .. )
        protected Vector3 m_targetPosition;
        
        // entity targeting ( can be ally or enemy for support or attack )
        protected AI m_targetAI;
        public AI targetAI => m_targetAI;

        protected virtual void Move()
        {
            if (m_targetAI != null)
            {
                agent.SetDestination(m_targetAI.transform.position);
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
                case States.Chasing:    DoChasing(); break;
                case States.Attacking:  DoAttacking(); break;
                case States.Flee:       DoFlee(); break;
                case States.Dead:       DoDead(); break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected virtual void DoIdle()
        { 
            m_animator.speed = 1f;
            m_animator.Play("Idle");
            CheckTargets();
        }

        protected virtual void DoWander()
        {
            m_animator.Play("Move" );
            m_animator.speed = agent.velocity.magnitude > 0.5f ? 1f : agent.velocity.magnitude;
            if (agent.remainingDistance < 0.5f)
            {
                agent.destination = new Vector3(m_positionInit.x + Random.Range(-m_magnitudeWander, m_magnitudeWander),
                    m_positionInit.y, m_positionInit.z + Random.Range(-m_magnitudeWander, m_magnitudeWander)); 
            }

            CheckTargets();
        }

        private void CheckTargets()
        {
            if (!m_detectionManager.TryGetFromType(m_currentTeam == Team.Hero ? Team.Orc : Team.Hero, out var target))
            {
                return;
            }
            m_targetAI = target;
            m_currentState = States.Chasing;
        }

        protected virtual void DoChasing()
        {
            if (m_targetAI == null)
            {
                m_currentState = States.Wander;
                return;
            }

            var distanceTarget = Vector3.Distance(transform.position, m_targetAI.transform.position);
            
            if (distanceTarget> m_radiusAgro + m_distanceForget)
            {
                m_targetAI = null;
                m_currentState = States.Wander;
                return;
            }

            if (distanceTarget < (m_currentJob == Jobs.Cac? m_distanceAttackCac: m_distanceAttackDistance))
            {
                m_animator.speed = 1f;
                m_animator.Play("Idle");

                m_currentState = States.Attacking;
                return;
            }
            
            Move();
        }

        private float m_lastAttack;
        
        protected virtual void DoAttacking()
        {
            if (Time.time - m_lastAttack > attackSpeedRate)
            {
                m_lastAttack = Time.time;
                m_animator.Play( m_currentJob ==Jobs.Cac ? "AttackCac" : "AttackDistance");
            }

            if (m_targetAI == null)
            {
                m_currentState = States.Wander;
                return;
            }
            
            var distanceTarget = Vector3.Distance(transform.position, m_targetAI.transform.position);

            if (distanceTarget > (m_currentJob == Jobs.Cac ? m_distanceAttackCac : m_distanceAttackDistance) +0.5f)
            {
                m_currentState = States.Chasing;
                return;
            }

            if (m_currentJob == Jobs.Shooter && distanceTarget < m_fleeTooNearAttackDistance)
            {
                m_currentState = States.Flee;
                return;
            }

            agent.SetDestination(transform.position);
        }

        protected virtual void DoDead()
        {
            // SPAWN FX
            Destroy(gameObject);
        }

        protected virtual void DoFlee()
        {
            if (m_targetAI == null)
            {
                m_currentState = States.Wander;
                return;
            }
            
            var distanceTarget = Vector3.Distance(transform.position, m_targetAI.transform.position);

            if (distanceTarget > (m_currentJob == Jobs.Cac ? m_distanceAttackCac : m_distanceAttackDistance) +0.5f)
            {
                m_currentState = States.Chasing;
                return;
            }
            
            if (distanceTarget > m_fleeTooNearAttackDistance && distanceTarget < (m_currentJob == Jobs.Cac? m_distanceAttackCac: m_distanceAttackDistance))
            {
                m_animator.speed = 1f;
                m_animator.Play("Idle");

                m_currentState = States.Attacking;
                return;
            }
            
            var fleeDestination = (transform.position- m_targetAI.transform.position).normalized;
            
            agent.SetDestination(fleeDestination);
        }

        public void Hit(int damages)
        {
            m_currentHp = Mathf.Max(0, m_currentHp-damages);
          
            FxManagers.RequestDamageFxAtPos(transform.position + Vector3.up );
            RefreshHp();
            
            if (m_currentHp == 0)
            {
                m_currentState = States.Dead;
            }
  
        }
        
        public void HitTarget()
        {
            if (m_targetAI == null)
            {
                return;
            }
            
            var distanceTarget = Vector3.Distance(transform.position, m_targetAI.transform.position);

            if (distanceTarget < m_distanceAttackDistance +1f)
            {
                m_targetAI.Hit(m_weapon != null ?m_weapon.damages : 1);
            }
        }

        public void RefreshHp()
        {
            m_aiHp.HpRatio((float)m_currentHp/m_baseHp);
        }
        
        #endregion

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, m_radiusAgro);
        }
    }
}
