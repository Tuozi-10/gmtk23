using System.Collections.Generic;
using Items;
using UI;
using UnityEngine;

public class Chest : MonoBehaviour {
    [SerializeField] private List<AbstractItem> items = new();
    [SerializeField] private GameObject itemGam = null;
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
        Inventory.instance.DropAbstractItem((- DashDir + Vector3.up).normalized * throwForce, transform.position, null, items[random]);

        Destroy(gameObject);
    }
}