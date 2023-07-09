using System;
using Gameplay;
using IAs;
using Items;
using UnityEngine;

public class ThrowItem : MonoBehaviour {
    [SerializeField] private AbstractItem item = null;
    public AbstractItem Item => item;
    [SerializeField] private SpriteRenderer itemSprite = null;
    private GameObject notGrabbableNow = null;
    
    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.CompareTag("Enemy") && notGrabbableNow != collision.gameObject) {
            if (item == null) return;

            bool hasGrabItem = true;
            if(item is Weapon weapon) hasGrabItem = collision.gameObject.GetComponent<AI>().SetWeapon(weapon);
            else if(item is Armor armor) collision.gameObject.GetComponent<AI>().SetArmor(armor);

            if (hasGrabItem) {
                PlayerController.instance.ResetPressEText();
                Destroy(gameObject);
            }
        }
    }

    /// <summary>
    /// Disable the possibility for an enemy to get his item back
    /// </summary>
    /// <param name="other"></param>
    private void OnCollisionExit(Collision other) {
        if (other.gameObject.CompareTag("Enemy") && notGrabbableNow == other.gameObject) {
            notGrabbableNow = null;
        }
    }

    /// <summary>
    /// Set the item of this object
    /// </summary>
    /// <param name="item"></param>
    public void SetItem(AbstractItem itemSet, GameObject notGrab) {
        itemSprite.sprite = itemSet.sprite;
        item = itemSet;
        notGrabbableNow = notGrab;
    }
}