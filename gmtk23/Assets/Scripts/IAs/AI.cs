using System;
using DG.Tweening;
using Gameplay.Projectiles;
using Items;
using Managers;
using UI;
using UnityEngine;
using UnityEngine.AI;
using static Items.Weapon;
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

        [SerializeField] private BodyType m_currentBodyType;

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

        [SerializeField] private States m_currentState = States.Idle;

        #endregion

        #region Team

        public enum Team
        {
            Orc,
            Hero
        }

        [SerializeField] private Team m_currentTeam = Team.Hero;
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
            Shooter,
        }

        [SerializeField] protected Jobs m_currentJob;

        #endregion

        #region Hp

        private int m_currentHp;
        [SerializeField] private int m_baseHp = 5;

        #endregion

        #region skill

        public enum Skills
        {
            NoSkill,
            Barbarian,
            Templar,
            Archer,
            Sorcier,
            Healer
        }

        private Skills m_skill;

        #endregion

        [Space, SerializeField] private float m_speedSmall = 3.5f;
        [SerializeField] private float m_speedMedium = 2.5f;
        [SerializeField] private float m_speedFat = 1.5f;

        [Space, SerializeField] private float m_magnitudeWander = 3;
        [Space, SerializeField] private float m_radiusAgro = 3;
        [SerializeField] private float m_distanceForget;
        [SerializeField] private SphereCollider m_triggerDetection;
        [SerializeField] private float m_distanceAttackCac = 1.5f;
        [SerializeField] private float m_distanceAttackDistance = 5.5f;
        [SerializeField] private float m_fleeTooNearAttackDistance = 3.5f;

        [SerializeField] private Arrow m_arrow;
        [SerializeField] private BulletTest m_fireball;

        [SerializeField] private AiHp m_aiHp;

        [SerializeField] private float attackSpeedRate = 1f;

        [SerializeField] private float damagesBonus = 1f;

        [SerializeField] private Transform m_body;
        public Transform body => m_body;

        [SerializeField] private Transform m_head;
        public Transform head => m_head;

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
        [SerializeField] private SpriteRenderer m_maskWeaponSlot;

        public bool SetWeapon(Weapon weapon)
        {
            if (!CanEquipWeapon(weapon))
            {
                return false;
            }

            m_weapon = weapon;
            RefreshStuffs();
            return true;
        }

        public void SetArmor(Armor armor)
        {
            m_armor = armor;
            RefreshStuffs();
        }

        public bool CanEquipWeapon(Weapon weapon)
        {
            return m_skill switch
            {
                Skills.Barbarian => weapon.weaponType is WeaponType.Axe or WeaponType.Bow or WeaponType.Kebab or
                    WeaponType.Sceptre,
                Skills.Templar => weapon.weaponType is WeaponType.Axe or WeaponType.Sword or WeaponType.Kebab or
                    WeaponType.Masse,
                Skills.Archer => weapon.weaponType is WeaponType.Bow or WeaponType.Baguette or WeaponType.Sword or
                    WeaponType.Sceptre,
                Skills.Sorcier => weapon.weaponType is WeaponType.Kebab or WeaponType.Baguette or WeaponType.Sceptre,
                Skills.Healer => weapon.weaponType is WeaponType.Bow or WeaponType.Baguette or WeaponType.Kebab or
                    WeaponType.Sceptre or WeaponType.Masse,
                _ => true
            };
        }

        public void RefreshStuffs()
        {
            m_armorSlot.sprite = m_armor == null ? null : m_armor.sprite;
            m_weaponSlot.sprite = m_weapon == null ? null : m_weapon.sprite;
            if (m_maskWeaponSlot != null) m_maskWeaponSlot.sprite = m_weapon == null ? null : m_weapon.sprite;

            if (m_armor != null)
            {
                m_skill = m_armor.m_skill;
            }

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
                case States.Idle:
                    DoIdle();
                    break;
                case States.Wander:
                    DoWander();
                    break;
                case States.Chasing:
                    DoChasing();
                    break;
                case States.Attacking:
                    DoAttacking();
                    break;
                case States.Flee:
                    DoFlee();
                    break;
                case States.Dead:
                    DoDead();
                    break;
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
            m_animator.Play("Move");
            m_animator.speed = agent.velocity.magnitude > 0.5f ? 1f : agent.velocity.magnitude;
            if (agent.remainingDistance < 0.5f)
            {
                agent.destination = new Vector3(m_positionInit.x + Random.Range(-m_magnitudeWander, m_magnitudeWander),
                    m_positionInit.y, m_positionInit.z + Random.Range(-m_magnitudeWander, m_magnitudeWander));
            }


            if (m_weapon is null)
            {
                CheckWeapons();
                return;
            }

            CheckTargets();
        }

        private void CheckWeapons()
        {
            // TODO
        }

        private void CheckTargets()
        {
            var desiredTeam = m_skill == Skills.Healer ? m_currentTeam == Team.Hero ? Team.Hero :
                Team.Orc :
                m_currentTeam == Team.Hero ? Team.Orc : Team.Hero;

            if (!m_detectionManager.TryGetFromType(desiredTeam, out var target, this))
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

            if (distanceTarget > m_radiusAgro + m_distanceForget)
            {
                m_targetAI = null;
                m_currentState = States.Wander;
                return;
            }

            if (distanceTarget < (!isDistance ? m_distanceAttackCac : m_distanceAttackDistance))
            {
                m_animator.speed = 1f;
                m_animator.Play("Idle");

                m_currentState = States.Attacking;
                return;
            }

            Move();
        }

        private bool isDistance => !isCacOnly && m_currentJob is Jobs.Shooter or Jobs.Support;
        private bool isCacOnly => m_skill is Skills.Barbarian or Skills.Templar or Skills.NoSkill;

        private float m_lastAttack;

        protected virtual void DoAttacking()
        {
            if (Time.time - m_lastAttack > attackSpeedRate)
            {
                m_lastAttack = Time.time;
                m_animator.Play(!isDistance ? "AttackCac" : "AttackDistance");
            }

            if (m_targetAI == null)
            {
                m_currentState = States.Wander;
                return;
            }

            var distanceTarget = Vector3.Distance(transform.position, m_targetAI.transform.position);

            if (distanceTarget > (!isDistance ? m_distanceAttackCac : m_distanceAttackDistance) + 0.5f)
            {
                m_currentState = States.Chasing;
                return;
            }

            if (isDistance && distanceTarget < m_fleeTooNearAttackDistance)
            {
                m_currentState = States.Flee;
                return;
            }

            agent.SetDestination(transform.position);
        }

        protected virtual void DoDead()
        {
            head.transform.SetParent(null);
            body.transform.SetParent(null);

            if (m_deadFromFire)
            {
                body.GetComponent<SpriteRenderer>().DOColor(new Color(0.3f, .3f, .3f, 255), 0.25f);
                head.GetComponent<SpriteRenderer>().DOColor(new Color(.3f, .3f, .3f, 255), 0.25f);
            }

            body.GetComponent<CapsuleCollider>().enabled = true;
            body.GetComponent<Rigidbody>().useGravity = true;

            head.GetComponent<CapsuleCollider>().enabled = true;
            head.GetComponent<Rigidbody>().useGravity = true;

            head.DOScale(0, 0.5f).SetDelay(3.5f).OnComplete(() => Destroy(head.gameObject));
            body.DOScale(0, 0.5f).SetDelay(3.5f).OnComplete(() => Destroy(body.gameObject));
            ;

            // SPAWN FX
            Destroy(m_weaponSlot.gameObject);
            Destroy(m_armorSlot.gameObject);
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
            
            if (m_weapon is not null)
            {
                if (distanceTarget > (!isDistance ? m_distanceAttackCac : m_distanceAttackDistance) + 0.5f)
                {
                    m_currentState = States.Chasing;
                    return;
                }

                if (distanceTarget > m_fleeTooNearAttackDistance &&
                    distanceTarget < (!isDistance ? m_distanceAttackCac : m_distanceAttackDistance))
                {
                    m_animator.speed = 1f;
                    m_animator.Play("Idle");

                    m_currentState = States.Attacking;
                    return;
                }
            }
            
            if (distanceTarget > m_distanceForget + 0.5f)
            {
                m_targetAI = null;
                return;
            }

            var fleeDestination = (transform.position - m_targetAI.transform.position).normalized;

            agent.SetDestination( transform.position  +fleeDestination);
        }

        private bool m_deadFromFire;

        public void Hit(int damages, bool fromFire = false)
        {
            m_currentHp = Mathf.Max(0, m_currentHp - damages);

            FxManagers.RequestDamageFxAtPos(transform.position + Vector3.up);
            RefreshHp();

            if (m_currentHp == 0)
            {
                m_deadFromFire = fromFire;
                AudioManager.PlaySoundRaleAgonie();
                FxManagers.RequestBloodFxAtPos(transform.position, team == Team.Orc && gameObject.name.Contains("Orc"));

                m_currentState = States.Dead;
            }
            else
            {
                AudioManager.PlaySoundHitMe();
            }
        }

        public void HitTarget()
        {
            if (m_targetAI == null)
            {
                return;
            }

            AudioManager.PlaySoundHitEnemy();

            if (isDistance && m_currentJob == Jobs.Shooter)
            {
                ShootArrow();
                return;
            }

            if (isDistance && m_currentJob == Jobs.Support)
            {
                ShootMagic();
                return;
            }

            var distanceTarget = Vector3.Distance(transform.position, m_targetAI.transform.position);

            if (distanceTarget < m_distanceAttackDistance + 1f)
            {
                m_targetAI.Hit(m_weapon != null ? (int) (m_weapon.damages * damagesBonus) : 1);
                if (m_weapon.weaponType is WeaponType.Baguette or WeaponType.Sceptre or WeaponType.Bow)
                {
                    // TODO BREAK FX
                    m_currentJob = Jobs.Cac;
                    m_weapon = null;
                    RefreshStuffs();
                    m_currentState = States.Flee;
                }
            }
        }

        public void ShootArrow()
        {
            if (targetAI == null)
            {
                return;
            }

            var arrow = Instantiate(m_arrow);
            arrow.transform.position = transform.position;
            arrow.SetUp(targetAI.transform.position - transform.position, team,
                (int) (m_weapon.damages * damagesBonus));
        }

        public void ShootMagic()
        {
            if (targetAI == null)
            {
                return;
            }

            var fireBall = Instantiate(m_fireball);
            fireBall.transform.position = transform.position + Vector3.up;
            fireBall.SetUp(targetAI, (int) (m_weapon.damages * damagesBonus));
        }

        public void RefreshHp()
        {
            m_aiHp.HpRatio((float) m_currentHp / m_baseHp);
        }

        #endregion

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, m_radiusAgro);
        }
    }
}