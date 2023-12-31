using System;
using System.Collections.Generic;
using DG.Tweening;
using Gameplay;
using Gameplay.Projectiles;
using Items;
using Managers;
using UI;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
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
            Stun,
            Dead
        }

        [SerializeField] protected States m_currentState = States.Idle;

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

        protected int m_currentHp;
        [SerializeField] protected int m_baseHp = 5;

        public bool FullLife => m_currentHp >= m_baseHp;
        
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

        #region pack

        public Pack currentPack;

        #endregion
        
        [Space, SerializeField] private float m_speedSmall = 3.5f;
        [SerializeField] private float m_speedMedium = 2.5f;
        [SerializeField] private float m_speedFat = 1.5f;

        [Space, SerializeField] private float m_magnitudeWander = 3;
        [Space, SerializeField] private float m_radiusAgro = 3;
        [SerializeField] private float m_distanceForget;
        [SerializeField] private SphereCollider m_triggerDetection;
        [SerializeField] private float m_distanceAttackCac = 1.5f;
        [SerializeField] protected float m_distanceAttackDistance = 5.5f;
        [SerializeField] protected float m_fleeTooNearAttackDistance = 3.5f;

        [SerializeField] private Arrow m_arrow;
        [SerializeField] private BulletTest m_fireball;
        [SerializeField] private BulletTest m_healBall;

        [SerializeField] private AiHp m_aiHp;

        [SerializeField] protected float attackSpeedRate = 1f;

        [SerializeField] private float damagesBonus = 1f;

        [SerializeField] private float scaleShockWave = 1;
        [SerializeField] private float m_stunMasseDuration = 1;

        [SerializeField] private Transform m_body;
        public Transform body => m_body;

        [SerializeField] private Transform m_head;
        public Transform head => m_head;

        private DetectionManager m_detectionManager;

        protected NavMeshAgent agent;

        [Space, SerializeField] protected Animator m_animator;
        [SerializeField] private GameObject stunFx;
        
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
            stunFx.SetActive(false);
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
        [SerializeField] private SpriteRenderer m_maskArmorSlot;

        private int m_level;
        private int level => Mathf.Clamp(m_level, 0, 2);
        
        public bool SetWeapon(Item weapon)
        {
            if (!CanEquipWeapon(weapon.item as Weapon)) {
                return false;
            }

            if (m_weapon != null && weapon != null && m_weapon.weaponType == (weapon.item as Weapon).weaponType) {
                m_level = Mathf.Clamp(m_level + 1, 0, 2);
            }
            else {
                if(m_weapon) Inventory.instance.DropAbstractItem(Vector3.zero, transform.position + transform.forward + Vector3.up, gameObject, new Item(m_level, m_weapon));
                m_level = weapon.level;
            }
            
            m_weapon = weapon.item as Weapon;
            RefreshStuffs();
            return true;
        }

        public void SetArmor(Armor armor)
        {
            Inventory.instance.DropAbstractItem(Vector3.zero, transform.position + transform.forward + Vector3.up, gameObject, new Item(0, m_armor));
            m_armor = armor;
            RefreshStuffs();
            
            if (m_weapon != null)
            {
                if (!CanEquipWeapon(m_weapon))
                {
                    RemoveWeapon();
                }
            }
        }

        public bool CanEquipWeaponWithSkill(Weapon weapon, Skills skill)
        {
            return skill switch
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
            if (m_armorSlot != null) {
                m_armorSlot.sprite = m_armor == null ? null : m_armor.sprite[0];
                if(m_maskArmorSlot != null) m_maskArmorSlot.sprite = m_armor == null ? null : m_armor.sprite[0];
            }
            
            m_weaponSlot.sprite = m_weapon == null ? null : m_weapon.sprite[m_level];
            if (m_maskWeaponSlot != null) m_maskWeaponSlot.sprite = m_weapon == null ? null : m_weapon.sprite[m_level];

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

        /// <summary>
        /// Remove a random item
        /// </summary>
        public void RemoveRandomItem() {
            List<AbstractItem> ToolsItemsList = new();
            if(m_weapon != null) ToolsItemsList.Add(m_weapon);
            if(m_armor != null) ToolsItemsList.Add(m_armor);

            if (ToolsItemsList.Count == 0) return;
            int random = Random.Range(0, ToolsItemsList.Count);
            Inventory.instance.DropAbstractItem(Vector3.zero, 
                transform.position - PlayerController.instance.DashDir * 2 + Vector3.up, gameObject, 
                new Item(ToolsItemsList[random] is Weapon ? m_level : 0, ToolsItemsList[random]));
            
            if(ToolsItemsList[random] is Weapon) RemoveWeapon();
            else RemoveArmor();
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
                case States.Idle: DoIdle(); break;
                case States.Wander: DoWander(); break;
                case States.Chasing: DoChasing(); break;
                case States.Attacking: DoAttacking(); break;
                case States.Flee: DoFlee(); break;
                case States.Stun: DoStun(); break;
                case States.Dead: DoDead(); break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private float timeEndStun;

        public void Stun(float duration)
        {
            if (m_currentHp <= 0 || m_currentState == States.Dead)
            {
                return;
            }
            timeEndStun = Time.time + duration;
            m_currentState = States.Stun;
            stunFx.SetActive(true);
        }
        
        protected virtual void DoStun()
        {
            if (Time.time > timeEndStun)
            {
                stunFx.SetActive(false);
                m_currentState = States.Wander;
            }
        }
        
        protected virtual void DoIdle()
        {
            m_animator.speed = 1f;
            m_animator.Play("Idle");
            
            if (m_weapon is null)
            {
                CheckWeapons();
                return;
            }
            
            CheckTargets();
        }

        private float wanderStartTime;
        
        protected virtual void DoWander()
        {
            m_animator.Play("Move");
            m_animator.speed = agent.velocity.magnitude > 0.5f ? 1f : agent.velocity.magnitude;
            if (agent.remainingDistance < 0.5f || Time.time -  wanderStartTime > 3f )
            {
                wanderStartTime = Time.time;
                Vector3 center = ( currentPack != null && currentPack.tracking != null) ? currentPack.tracking.transform.position : m_positionInit;
                
                agent.destination = new Vector3(center.x + Random.Range(-m_magnitudeWander, m_magnitudeWander),
                    center.y, center.z + Random.Range(-m_magnitudeWander, m_magnitudeWander));
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
            // TODO collect weapons
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

        protected bool isDistance => !isCacOnly && m_currentJob is Jobs.Shooter or Jobs.Support;
        private bool isCacOnly => m_skill is Skills.Barbarian or Skills.Templar or Skills.NoSkill;

        protected float m_lastAttack;

        protected virtual void DoAttacking()
        {
            if (Time.time - m_lastAttack > attackSpeedRate)
            {
                m_lastAttack = Time.time;
                m_animator.Play(!isDistance ? "AttackCac" : "AttackDistance");
            }

            if (m_targetAI == null || (m_targetAI.m_currentHp >= m_targetAI.m_baseHp && isHealer))
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
            if (head == null)
            {
                return;
            }
            
            head.transform.SetParent(null);
            body.transform.SetParent(null);
            body.gameObject.AddComponent<SortingGroup>().sortingLayerName = "Middle";
            head.gameObject.AddComponent<SortingGroup>().sortingLayerName = "Middle";
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
            
            if (currentPack != null && currentPack.packMobManagerLink != null) currentPack.packMobManagerLink.JsuisDead();

            // SPAWN FX
            Destroy(m_weaponSlot.gameObject);
            if (m_armorSlot != null)
            {
                Destroy(m_armorSlot.gameObject);
            }

            Destroy(gameObject);
        }

        protected bool isHealer =>
            m_skill is Skills.Healer && m_weapon != null && m_weapon.weaponType == WeaponType.Sceptre;
        
        protected virtual void DoFlee()
        {
            if (m_targetAI == null )
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
            m_currentHp = Mathf.Clamp(m_currentHp - damages,0, m_baseHp);

            if (damages > 0)
            {
                FxManagers.RequestDamageFxAtPos(transform.position + Vector3.up);
            }
            else
            {
                FxManagers.RequestHealFxAtPos(transform.position + Vector3.up);
            }
            
            RefreshHp();

            if (m_currentHp == 0)
            {
                m_deadFromFire = fromFire;
                AudioManager.PlaySoundRaleAgonie();

                var orc = gameObject.name.Contains("Orc");
                if (orc || team == Team.Hero)
                {
                    AudioManager.PlaySoundHitFlesh();
                }
                else
                {
                    AudioManager.PlaySoundHitBones();
                }
                
                FxManagers.RequestBloodFxAtPos(transform.position, team == Team.Orc && orc);

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
                if (m_weapon.weaponType == WeaponType.Baguette)
                {
                    ShootMagic();
                }
                else
                {
                    ShootHeal();
                }
                return;
            }

            var distanceTarget = Vector3.Distance(transform.position, m_targetAI.transform.position);

            if (distanceTarget < m_distanceAttackDistance + 1f)
            {
                m_targetAI.Hit(m_weapon != null ? (int) (m_weapon.damages[level] * damagesBonus) : 1);

                if (m_weapon.weaponType == Weapon.WeaponType.Axe && m_skill == Skills.Barbarian)
                {
                    FxManagers.RequestHitShockWaveFxAtPos(m_weaponSlot.transform.position, scaleShockWave, m_currentTeam  == Team.Hero ? Team.Orc : Team.Hero, (int) (m_weapon.damages[level] * damagesBonus));
                }
                else if (m_weapon.weaponType == WeaponType.Masse && m_skill == Skills.Templar)
                {     
                    AudioManager.PlaySoundAoE();
                    FxManagers.RequestHitShockWaveFxAtPos(m_weaponSlot.transform.position, scaleShockWave, m_currentTeam  == Team.Hero ? Team.Orc : Team.Hero, (int) (m_weapon.damages[level] * damagesBonus));
                    m_targetAI.Stun(m_stunMasseDuration);
                }
                else if (m_weapon.weaponType is WeaponType.Baguette or WeaponType.Sceptre or WeaponType.Bow)
                {
                    m_currentJob = Jobs.Cac;
                    RemoveWeapon();
                    m_currentState = States.Flee;
                }
            }
        }

        public void RemoveWeapon()
        {
            m_currentJob = Jobs.Cac;
            m_weapon = null;
            RefreshStuffs();
            m_currentState = States.Wander;
        }
        
        public void RemoveArmor()
        {
            m_skill = Skills.NoSkill;
            m_armor = null;
            RefreshStuffs();
        }

        public void ShootArrow()
        {
            if (targetAI == null)
            {
                return;
            }

            AudioManager.PlaySoundShootArrow();
            
            var arrow = Instantiate(m_arrow);
            arrow.transform.position = transform.position;
            arrow.SetUp(targetAI.transform.position - transform.position, team,
                (int) (m_weapon.damages[level] * damagesBonus));
        }

        public void ShootMagic()
        {
            if (targetAI == null)
            {
                return;
            }

            AudioManager.PlaySoundFireBall();
            
            var fireBall = Instantiate(m_fireball);
            fireBall.transform.position = transform.position + Vector3.up;
            fireBall.SetUp(targetAI, (int) (m_weapon.damages[level] * damagesBonus));
        }
        
        public void ShootHeal()
        {
            if (targetAI == null)
            {
                return;
            }

            var fireBall = Instantiate(m_healBall);
            fireBall.transform.position = transform.position + Vector3.up;
            fireBall.SetUp(targetAI, (int) (m_weapon.damages[level] * damagesBonus), m_skill == Skills.Sorcier);
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