using System;
using Gameplay;
using UnityEngine;

public class PlayerDetection : MonoBehaviour {
    
    /// <summary>
    /// When colliding with enemy
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other) {
        if (PlayerController.instance.CurrentDashState == DashState.isInDash) {
            if (other.gameObject.CompareTag("Enemy")) {
                other.TryGetComponent(out StockRemove stock);
                PlayerController.instance.StopMovement();
                PlayerController.instance.StartCollidingWithEnemy(stock);
            }
            else if (other.gameObject.CompareTag("Chest")) {
                PlayerController.instance.StopMovement();
                other.GetComponent<Chest>().DropRandomItem(PlayerController.instance.DashDir);
            }
        }
        else if (other.gameObject.CompareTag("Catchable")) {
            PlayerController.instance.ChangeObjectState(other.GetComponent<ThrowItem>().Item, other.gameObject, true);
        }
    }

    /// <summary>
    /// When exiting the collision of an enemy
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other) {
        if (other.gameObject.CompareTag("Catchable")) {
            PlayerController.instance.ChangeObjectState(other.GetComponent<ThrowItem>().Item, other.gameObject, false);
        }
    }
}