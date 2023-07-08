using src.Singletons;
using UnityEngine;

namespace Gameplay {
    public class PlayerController : MonoSingleton<PlayerController> {
        private Rigidbody rb;
        private PlayerMap inputs;
        
        [Header("Player Movement")]
        [SerializeField, Range(0,30)] private float moveSpeed = 2f;
        [SerializeField, Range(0, 30)] private float drag = 15f;
        private Vector2 dir = new();
        
        private void Start() {
            rb = GetComponent<Rigidbody>();
            rb.drag = drag;
            inputs = new PlayerMap();
            inputs.Enable();
        }

        private void Update() {
            GetCurrentPlayerVelocity();
        }

        /// <summary>
        /// Method called each fixed frame of Unity (allow better movement and physics calc)
        /// </summary>
        private void FixedUpdate() {
            if(dir.magnitude != 0) rb.velocity = new Vector3(dir.x, 0, dir.y);
        }

        /// <summary>
        /// Get the current velocity of the player based on the inputs
        /// </summary>
        private void GetCurrentPlayerVelocity() {
            dir = inputs.Movement.MoveDirection.ReadValue<Vector2>().normalized * moveSpeed;
        }
    }
}