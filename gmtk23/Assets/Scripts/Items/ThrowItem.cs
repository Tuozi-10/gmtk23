using Gameplay;
using IAs;
using Items;
using UnityEngine;

public class ThrowItem : MonoBehaviour {
    [SerializeField] private Item item = null;
    public Item Item => item;
    [SerializeField] private SpriteRenderer itemSprite = null;
    private GameObject notGrabbableNow = null;
    public bool canLevelUp = true;

    private void Start() {
        canLevelUp = true;
    }

    /// <summary>
    /// When object enter in collision with something
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter(Collider collision) {
        if (collision.gameObject.CompareTag("Enemy") && notGrabbableNow != collision.gameObject) {
            if (item == null) return;

            bool hasGrabItem = true;
            if(item.item is Weapon) hasGrabItem = collision.gameObject.GetComponent<AI>().SetWeapon(item);
            else if(item.item is Armor armor) collision.gameObject.GetComponent<AI>().SetArmor(armor);

            if (hasGrabItem) {
                PlayerController.instance.ResetPressEText();
                Destroy(gameObject);
            }
        }
        else if (collision.gameObject.CompareTag("Catchable") && canLevelUp && collision.gameObject.GetComponent<ThrowItem>().item.item == item.item && item.level < 2) {
            ThrowItem itemTh = collision.gameObject.GetComponent<ThrowItem>();
            itemTh.canLevelUp = false;
            
            item.level = Mathf.Clamp(item.level + 1, 0, 2);
            itemSprite.sprite = item.item.sprite[Mathf.Clamp(item.level, 0, item.item.sprite.Count - 1)];
            if(PlayerController.instance.TextGetItem.transform.parent == collision.transform) PlayerController.instance.ResetPressEText();
            Destroy(collision.gameObject);
        }
    }

    /// <summary>
    /// Disable the possibility for an enemy to get his item back
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other) {
        if (other.gameObject.CompareTag("Enemy") && notGrabbableNow == other.gameObject) {
            notGrabbableNow = null;
        }
    }

    /// <summary>
    /// Set the item of this object
    /// </summary>
    /// <param name="item"></param>
    public void SetItem(Item itemSet, GameObject notGrab) {
        item = itemSet;
        itemSprite.sprite = item.item.sprite[Mathf.Clamp(item.level, 0, item.item.sprite.Count - 1)];
        notGrabbableNow = notGrab;
    }
}

[System.Serializable]
public class Item {
    public AbstractItem item;
    public int level;

    public Item(int level, AbstractItem item) {
        this.level = level;
        this.item = item;
    }
}