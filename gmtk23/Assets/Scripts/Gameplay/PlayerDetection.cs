using Gameplay;
using UnityEngine;

public class PlayerDetection : MonoBehaviour {
    
    /// <summary>
    /// When colliding with enemy
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Enemy") && PlayerController.instance.CurrentDashState == DashState.isInDash) {
            other.TryGetComponent(out StockRemove stock);
            PlayerController.instance.StartCollidingWithEnemy(stock);
        }
    }
}