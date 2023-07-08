using Gameplay;
using UnityEngine;

public class PlayerDetection : MonoBehaviour {
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Enemy") && PlayerController.instance.CurrentDashState == DashState.isInDash) {
            PlayerController.instance.StartCollidingWithEnemy();
        }
    }
}