using IAs;
using Items;
using UnityEngine;

public class ThrowItem : MonoBehaviour {
    [SerializeField] private AbstractItem item = null;
    public AbstractItem Item => item;
    [SerializeField] private SpriteRenderer itemSprite = null;

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.CompareTag("Enemy")) {
            if (item == null) return;
            
            if(item is Weapon weapon) collision.gameObject.GetComponent<AI>().SetWeapon(weapon);
            else if(item is Armor armor) collision.gameObject.GetComponent<AI>().SetArmor(armor);
            
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Set the item of this object
    /// </summary>
    /// <param name="item"></param>
    public void SetItem(AbstractItem itemSet) {
        itemSprite.sprite = itemSet.sprite;
        item = itemSet;
    }
}