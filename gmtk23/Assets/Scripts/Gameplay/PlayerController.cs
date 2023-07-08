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

        [Header("Dash")]
        [SerializeField, ReadOnly] private DashState currentDashState = DashState.none;
        [SerializeField] private float dashForce = 10f;
        [SerializeField, Range(0,5)] private float dashCdTime = 1f;
        [SerializeField] private float timeInDash = 1f;
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
                
                case DashState.WaitForDash when dir.magnitude != 0 && velocityChangeTime < decelerationDashTime:
                    rb.velocity = Vector3.Lerp(startVelocity, new Vector3(dir.x, 0, dir.y) * moveSpeed, velocityChangeTime / decelerationDashTime);
                    velocityChangeTime += Time.fixedDeltaTime;
                    break;
                
                default:{
                    if (currentDashState != DashState.isInDash && dir.magnitude != 0 && velocityChangeTime >= decelerationDashTime) {
                        rb.velocity = new Vector3(dir.x, 0, dir.y) * moveSpeed;
                    }
                    else if (currentDashState != DashState.isInDash && dir.magnitude == 0) {
                        startVelocity = Vector3.zero;
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

        /// <summary>
        /// Make the player dash
        /// </summary>
        /// <param name="obj"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void PerformDash(InputAction.CallbackContext obj) {
            if (currentDashState != DashState.none) return;
            playerCam.ChangeDashState(true);
            DashDir = GetMouseDirFromPlayer();
            currentDashState = DashState.isInDash;
            timeSinceStartDash = 0;
            timeSinceLastDash = 0;
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


        private void OnTriggerEnter(Collider other) {
            Debug.Log(other.gameObject.name);
        }

        #region UI
        /// <summary>
        /// Update the dash interface of the player
        /// </summary>
        private void UpdateDashCooldownUI(bool dashEnable = false) {
            dashCooldownImg.color = startDashColor;
            dashCooldownImg.fillAmount = timeSinceLastDash / dashCdTime;
            if (dashEnable) {
                dashCooldownParent.DOPunchScale(new Vector3(.75f, .75f, .75f), getDashAnimDuration, 1);
                dashCooldownImg.DOColor(hasDashColor, getDashAnimDuration);
            }
        }
        #endregion UI
        
        #region Helper
        /// <summary>
        /// Get the current velocity of the player based on the inputs
        /// </summary>
        private void GetCurrentPlayerVelocity() => dir = inputs.Movement.MoveDirection.ReadValue<Vector2>().normalized;
        
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