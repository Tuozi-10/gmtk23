using System;
using DG.Tweening;
using src.Singletons;
using UnityEngine;
using UnityEngine.InputSystem;
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
        [SerializeField, Range(0,2)] private float accelerationTime = .2f;
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
        private Vector3 DashDir = new();
        private float timeSinceStartDash = 0;
        private float timeSinceLastDash = 0;
        
        [Header("Interface")]
        [SerializeField] private Transform dashCooldownParent = null;
        [SerializeField] private Image dashCooldownImg = null;
        [SerializeField] private Color startDashColor = new();
        [SerializeField] private Color hasDashColor = new();
        [SerializeField] private float getDashAnimDuration = 0.25f;
        
        private void Start() {
            cam = Camera.main;
            playerCam = cam.GetComponent<PlayerCamera>();
            rb = GetComponent<Rigidbody>();
            rb.drag = drag;
            velocityChangeTime = decelerationDashTime;

            inputs = new PlayerMap();
            inputs.Enable();
            inputs.Movement.Dash.started += PerformDash;
        }
        
        private void Update() {
            GetCurrentPlayerVelocity();
            UpdateDashState();
        }

        /// <summary>
        /// Method called each fixed frame of Unity (allow better movement and physics calc)
        /// </summary>
        private void FixedUpdate() {
            SetPlayerVelocity();
        }
        
        /// <summary>
        /// Move the player (movement, dash, ...)
        /// </summary>
        private void SetPlayerVelocity() {
            switch (currentDashState) {
                case DashState.isInDash:
                    rb.velocity = DashDir * dashForce;
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
            
            DashDir = GetMouseDirFromPlayer();
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

        /// <summary>
        /// Method called when colliding with an enemy
        /// </summary>
        public void StartCollidingWithEnemy(StockRemove enemy) {
            GiveDashToPlayer();
            rb.velocity = Vector3.zero;
            movementDisable = moveDisableAfterCollidingInDash;
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

        private void DamageEnemy() {
            if(enemyDamageable != null) enemyDamageable.ApplyStock();
            enemyDamageable = null;
        }

        #region UI
        /// <summary>
        /// Update the dash interface of the player
        /// </summary>
        private void UpdateDashCooldownUI(bool dashEnable = false) {
            dashCooldownImg.color = startDashColor;
            dashCooldownImg.fillAmount = timeSinceLastDash / dashCdTime;
            if (dashEnable) {
                dashCooldownParent.DORewind();
                dashCooldownParent.DOPunchScale(new Vector3(.75f, .75f, .75f), getDashAnimDuration, 1);
                dashCooldownImg.DOColor(hasDashColor, getDashAnimDuration);
            }
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
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            return Physics.Raycast(ray, out RaycastHit hit, 100, groundLayer) ? (hit.point - transform.position).normalized : Vector3.zero;
        }
        #endregion Helper
    }

    public enum DashState {
        none,
        isInDash,
        WaitForDash
    }
}