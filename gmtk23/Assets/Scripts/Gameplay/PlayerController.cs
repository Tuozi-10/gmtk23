using System;
using DG.Tweening;
using Items;
using Managers;
using src.Singletons;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Gameplay {
    public class PlayerController : MonoSingleton<PlayerController> {
        [SerializeField] private LayerMask groundLayer = new();
        private PlayerCamera playerCam;
        private PlayerMap inputs;
        private Rigidbody rb;
        public Rigidbody Rb => rb;
        private Camera cam;
        
        [FormerlySerializedAs("acceleration")]
        [Header("Player Movement")]
        [SerializeField, Range(0,30)] private float moveSpeed = 2f;
        [SerializeField, Range(0,3)] private float decelerationDashTime = 2f;
        [SerializeField, Range(0,30)] private float drag = 15f;
        private float velocityChangeTime = 0;
        private Vector2 dir = new();
        private Vector3 startVelocity = new();
        private float movementDisable = 0;

        [Header("Dash")]
        [SerializeField, ReadOnly] private DashState currentDashState = DashState.none;
        public DashState CurrentDashState => currentDashState;
        [SerializeField] private float dashForce = 10f;
        [SerializeField, Range(0,5)] private float dashCdTime = 1f;
        [SerializeField] private float timeInDash = 1f;
        [SerializeField] private float moveDisableAfterCollidingInDash = .2f;
        private StockRemove enemyDamageable = null;
        private Vector3 dashDir = new();
        public Vector3 DashDir => dashDir;
        private float timeSinceStartDash = 0;
        private float timeSinceLastDash = 0;
        [SerializeField] private ParticleSystem dashWalkEffect;

        [Header("Throw object")]
        [SerializeField] private Transform cursorTransform = null;
        [SerializeField] private GameObject itemGam = null;
        [SerializeField] private float throwForce = 0;
        private ThrowItem throwItem = null;

        [Header("Inventory")]
        [SerializeField] private Inventory playerInv = new();
        [SerializeField] private GameObject textGetItem = null;
        public GameObject TextGetItem => textGetItem;
        
        [Header("Interface")]
        [SerializeField] private Transform dashCooldownParent = null;
        [SerializeField] private Image dashCooldownImg = null;
        [SerializeField] private Color startDashColor = new();
        [SerializeField] private Color hasDashColor = new();
        [SerializeField] private float getDashAnimDuration = 0.25f;

        [SerializeField] private Transform ParentBody;
        [SerializeField] private Animator m_animator;
        
        private SortingGroup _sortingGroup;
        private Vector3 m_spawnPosition;
        
        private void Start() {
            cam = Camera.main;
            playerCam = cam.GetComponent<PlayerCamera>();
            rb = GetComponent<Rigidbody>();
            rb.drag = drag;
            velocityChangeTime = decelerationDashTime;
            textGetItem.GetComponent<CanvasGroup>().alpha = 0;
            m_spawnPosition = transform.position;
            
            inputs = new PlayerMap();
            inputs.Enable();
            inputs.Movement.Dash.started += PerformDash;
            inputs.Movement.ThrowItem.started += ThrowItem;
            inputs.Movement.Interact.started += AddItemToInventory;
            m_currentHp = hp;
            _sortingGroup = GetComponent<SortingGroup>();
            m_uiHp.HpRatio(1);
        }

        public void Respawn()
        {
            m_currentHp = hp;
            m_uiHp.HpRatio(1);
           GetComponent<Rigidbody>() .position = m_spawnPosition;
        }
        
        private void Update() {
            GetCurrentPlayerVelocity();
            SetCursorLocation();
            UpdateDashState();
            _sortingGroup.sortingOrder = -(int)(transform.position.z*30);
            if(rb.velocity.magnitude > 0.2f)
            ParentBody.localScale = new Vector3(rb.velocity.x > 0 ? 1 : -1, 1, 1);
            m_animator.Play(rb.velocity.magnitude > 0.65f ? "Move": "Idle");
        }

        /// <summary>
        /// Method called each fixed frame of Unity (allow better movement and physics calc)
        /// </summary>
        private void FixedUpdate() {
            SetPlayerVelocity();
        }
        
        #region Velocity
        /// <summary>
        /// Move the player (movement, dash, ...)
        /// </summary>
        private void SetPlayerVelocity() {
            switch (currentDashState) {
                case DashState.isInDash:
                    rb.velocity = dashDir * dashForce;
                    break;
                
                case DashState.WaitForDash when velocityChangeTime < decelerationDashTime:
                    rb.velocity = Vector3.Lerp(startVelocity, new Vector3(dir.x, 0, dir.y) * moveSpeed, velocityChangeTime / decelerationDashTime);
                    velocityChangeTime += Time.fixedDeltaTime;
                    break;
                
                default:{
                    if (currentDashState == DashState.isInDash) return;
                    
                    //If the player has decelerate to the current velocity value
                    if (velocityChangeTime >= decelerationDashTime) {
                        if (movementDisable <= 0) rb.velocity = new Vector3(dir.x, 0, dir.y) * moveSpeed;
                        else {
                            movementDisable -= Time.fixedDeltaTime;
                            if(movementDisable <= 0) ExitSlowMotion();
                        }
                    }
                    break;
                }
            }
        }

        /// <summary>
        /// Start decelerating from dash
        /// </summary>
        private void SetStartVelocity() {
            velocityChangeTime = 0;
            startVelocity = rb.velocity;
        }
        #endregion Velocity

        #region Dash
        /// <summary>
        /// Make the player dash
        /// </summary>
        /// <param name="obj"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void PerformDash(InputAction.CallbackContext obj) {
            if (currentDashState != DashState.none) return;
            ExitSlowMotion();
            playerCam.ChangeDashState(true);
            movementDisable = 0;
            FxManagers.RequestDashFxAtPos(transform.position+Vector3.up);
            AudioManager.PlaySoundDash();
            dashDir = GetMouseDirFromPlayer();
            timeSinceStartDash = 0;
            timeSinceLastDash = 0;
            currentDashState = DashState.isInDash;
            UpdateDashCooldownUI();
        }
        
        /// <summary>
        /// Update the current state of the dash
        /// </summary>
        private void UpdateDashState() {
            switch (currentDashState) {
                case DashState.isInDash:
                    timeSinceStartDash += Time.deltaTime;
                    if (timeSinceStartDash >= timeInDash) {
                        playerCam.ChangeDashState(false);
                        SetStartVelocity();
                        currentDashState = DashState.WaitForDash;
                    }
                    break;

                case DashState.WaitForDash:
                    timeSinceLastDash += Time.deltaTime;
                    UpdateDashCooldownUI();

                    if (timeSinceLastDash >= dashCdTime) {
                        currentDashState = DashState.none;
                        UpdateDashCooldownUI(true);
                    }
                    break;
            }
        }


        public void StopMovement() {
            rb.velocity = Vector3.zero;
            movementDisable = moveDisableAfterCollidingInDash;
        }
        
        
        /// <summary>
        /// Method called when colliding with an enemy
        /// </summary>
        public void StartCollidingWithEnemy(StockRemove enemy) {
            GiveDashToPlayer();
            if(enemy != null) enemyDamageable = enemy;
            if (movementDisable == 0) DamageEnemy();
        }

        /// <summary>
        /// Give the dash back to the player
        /// </summary>
        private void GiveDashToPlayer() {
            timeSinceStartDash = timeInDash;
            timeSinceLastDash = dashCdTime;
            currentDashState = DashState.none;
            UpdateDashCooldownUI(true);
        }
        #endregion

        #region Slow motion
        /// <summary>
        /// Start slow motion
        /// </summary>
        private void StartSlowMotion() {
            TimeManager.instance.StartSlowMotion();
            playerCam.ChangeSlowMo(true);
        }

        /// <summary>
        /// End slow motion
        /// </summary>
        private void ExitSlowMotion() {
            TimeManager.instance.EndSlowMotion();
            playerCam.ChangeSlowMo(false);
            DamageEnemy();
        }
        #endregion Slow motion
        
        #region Throw Object
        /// <summary>
        /// Throw the current item
        /// </summary>
        /// <param name="obj"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void ThrowItem(InputAction.CallbackContext obj) {
            if (playerInv.items.Count <= playerInv.CurrentSelectedSlot) return;
            Inventory.instance.DropAbstractItem(CalculateVelocity(), transform.position, null, playerInv.items[playerInv.CurrentSelectedSlot]);
            playerInv.RemoveItem();
        }

        /// <summary>
        /// Calculate the velocity of the object
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        private Vector3 CalculateVelocity() {
            Vector3 distance = GetMousePosition() - transform.position;
            float time = distance.magnitude / throwForce;
            
            float VelocityX = distance.magnitude / time;
            float VelocityY = distance.y / time + 0.5f * Mathf.Abs(Physics.gravity.y) * time;
            
            Vector3 finalVel = distance.normalized;
            finalVel *= VelocityX;
            finalVel.y = VelocityY;
            return finalVel;
        }

        /// <summary>
        /// Change Object State
        /// </summary>
        /// <param name="catchable"></param>
        /// <param name="add"></param>
        public void ChangeObjectState(ThrowItem throwI, bool add) {
            if (add) {
                throwItem = throwI;
                textGetItem.transform.parent = throwI.transform;
                textGetItem.transform.localPosition = Vector3.up;
                textGetItem.GetComponent<CanvasGroup>().DOFade(1, 0.25f);
            }
            else if (throwItem == throwI) {
                throwItem = null;
                textGetItem.GetComponent<CanvasGroup>().DOFade(0, 0.25f).OnComplete(() => textGetItem.transform.parent = null);
            }
        }

        public void ResetPressEText() {
            throwItem = null;
            textGetItem.transform.parent = null;
            textGetItem.GetComponent<CanvasGroup>().alpha = 0;
        }

        /// <summary>
        /// Add the item in your inventory
        /// </summary>
        /// <param name="obj"></param>
        private void AddItemToInventory(InputAction.CallbackContext obj) {
            if (throwItem == null) return;
            throwItem = playerInv.TryAddItem(throwItem.Item, throwItem);

            if (throwItem == null) ResetPressEText();
        }
        #endregion Throw Object
        
        #region UI
        /// <summary>
        /// Update the dash interface of the player
        /// </summary>
        private void UpdateDashCooldownUI(bool dashEnable = false) {
            dashCooldownImg.color = startDashColor;
            dashCooldownImg.fillAmount = timeSinceLastDash / dashCdTime;
            var mainModule = dashWalkEffect.main;
            mainModule.startColor = new ParticleSystem.MinMaxGradient(Color.Lerp(hasDashColor,Color.white, dashCooldownImg.fillAmount));
            if (dashEnable) {
                dashCooldownParent.DORewind();
                dashCooldownParent.DOPunchScale(new Vector3(.75f, .75f, .75f), getDashAnimDuration, 1);
                dashCooldownImg.DOColor(hasDashColor, getDashAnimDuration);
            }
        }
        
        /// <summary>
        /// Set the position of the cursor to the mouse position
        /// </summary>
        private void SetCursorLocation() {
            cursorTransform.position = GetMousePosition() + new Vector3(0, 0.1f, 0);
        }
        #endregion UI
        
        #region Helper
        /// <summary>
        /// Get the current velocity of the player based on the inputs
        /// </summary>
        private void GetCurrentPlayerVelocity() => dir = movementDisable > 0 ? Vector2.zero : inputs.Movement.MoveDirection.ReadValue<Vector2>().normalized;
        
        /// <summary>
        /// Get the Vector direction between the mouse and the player
        /// </summary>
        /// <returns></returns>
        private Vector3 GetMouseDirFromPlayer() {
            return (GetMousePosition() - transform.position).normalized;
        }

        /// <summary>
        /// Get the position of the mouse on the ground
        /// </summary>
        /// <returns></returns>
        private Vector3 GetMousePosition() {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            return Physics.Raycast(ray, out RaycastHit hit, 100, groundLayer) ? hit.point : Vector3.zero;
        }
        
        /// <summary>
        /// Damage the enemy after some time
        /// </summary>
        private void DamageEnemy() {
            if(enemyDamageable != null) enemyDamageable.ApplyStock();
            enemyDamageable = null;
        }
        #endregion Helper

        #region hp

        [SerializeField] private AiHp m_uiHp;

        public int hp = 10;

        public int m_currentHp;

        public void Hit(int damages)
        {
            m_currentHp -= damages;

            m_currentHp = Mathf.Clamp(m_currentHp, 0, hp);
            m_uiHp.HpRatio((float)m_currentHp / hp);
            if (m_currentHp <= 0)
            {
                Die();
            }
        }

        public void Die()
        {
            TesMortManager.Respawn();
        }
        
        #endregion

    }

    public enum DashState {
        none,
        isInDash,
        WaitForDash
    }
}