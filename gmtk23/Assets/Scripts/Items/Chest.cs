using System.Collections.Generic;
using Items;
using UI;
using UnityEngine;

public class Chest : MonoBehaviour {
    [SerializeField] private List<AbstractItem> items = new();
    [SerializeField] private Sprite chestOpen = null;
    [SerializeField] private Collider col = null;
    [SerializeField] private float throwForce = 1000f;
    private bool hasSpawn = false;
    
    /// <summary>
    /// Drop an object
    /// </summary>
    /// <param name="DashDir"></param>
    public void DropRandomItem(Vector3 DashDir) {
        if (hasSpawn) return;
        hasSpawn = true;
        
        int random = Random.Range(0, items.Count);
        col.enabled = false;
        Inventory.instance.DropAbstractItem((-transform.forward + Vector3.up) * throwForce, transform.position, null, new Item(0, items[random]));
        GetComponentInChildren<SpriteRenderer>().sprite = chestOpen;
    }
}