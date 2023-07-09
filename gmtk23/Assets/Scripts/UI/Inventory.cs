using System.Collections.Generic;
using DG.Tweening;
using Gameplay;
using Items;
using src.Singletons;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.UI;

namespace UI
{
    public class Inventory : MonoSingleton<Inventory> {
        [SerializeField] private GameObject itemGam = null;
        
        public List<ICatchable> items = new();
        [SerializeField] private List<RectTransform> slots = new();
        [SerializeField] private List<Image> slotsImg = new();
        [SerializeField] private RectTransform selectedSlot = new();
        [SerializeField, ReadOnly] private int currentSelectedSlot = 0;
        public int CurrentSelectedSlot => currentSelectedSlot;
        [SerializeField] private Vector2 slotSizeSelected = new(90, 90);
        [SerializeField] private Vector2 slotSizeUnSelected = new(70, 70);
        private PlayerMap inputs;

        public AbstractItem addItem = null;
        
        /// <summary>
        /// Init value
        /// </summary>
        private void Start() {
            foreach (var tr in slots) {
                tr.sizeDelta = slotSizeUnSelected;
            }

            slots[0].sizeDelta = slotSizeSelected;
            selectedSlot.transform.position = slots[0].position;
            UpdateInventoryUI();

            inputs = new PlayerMap();
            inputs.Enable();
            inputs.Inventory.ScrollInventory.started += _ => ChangeCurrentSelectedSlotValue(_.ReadValue<float>() > 0);
            inputs.Inventory.Slot1.started += _ => GoToSlot(0);
            inputs.Inventory.Slot2.started += _ => GoToSlot(1);
            inputs.Inventory.Slot3.started += _ => GoToSlot(2);
            inputs.Inventory.Slot4.started += _ => GoToSlot(3);
            inputs.Inventory.Slot5.started += _ => GoToSlot(4);
            inputs.Inventory.Slot6.started += _ => GoToSlot(5);
        }

        /// <summary>
        /// Update the position of the selected inventory
        /// </summary>
        private void Update() {
            selectedSlot.transform.position = Vector3.Lerp(selectedSlot.transform.position, slots[currentSelectedSlot].position, 0.1f);
        }

        /// <summary>
        /// Change the currently selected slots
        /// </summary>
        private void ChangeCurrentSelectedSlotValue(bool positive) {
            int lastSlotValue = currentSelectedSlot;
            
            currentSelectedSlot += positive ? 1 : -1;
            if (currentSelectedSlot >= slots.Count) currentSelectedSlot = 0;
            else if (currentSelectedSlot < 0) currentSelectedSlot = slots.Count - 1;
            
            UpdateSelectedSlotUI(lastSlotValue);
        }

        /// <summary>
        /// Go to a specific slot
        /// </summary>
        /// <param name="value"></param>
        private void GoToSlot(int value) {
            int lastSlotValue = currentSelectedSlot;
            currentSelectedSlot = value;
            UpdateSelectedSlotUI(lastSlotValue);
        }
        
        
        
        /// <summary>
        /// Change the current selected slot
        /// </summary>
        /// <param name="newSlot"></param>
        private void UpdateSelectedSlotUI(int lastSlot) {
            slots[lastSlot].DORewind();
            slots[currentSelectedSlot].DORewind();

            Sequence seq = DOTween.Sequence();

            seq.Append(slots[lastSlot].DOSizeDelta(slotSizeUnSelected, 0.25f));
            seq.Join(slots[currentSelectedSlot].DOSizeDelta(slotSizeSelected, 0.25f));
        }

        /// <summary>
        /// Update the slots of the inventory
        /// </summary>
        private void UpdateInventoryUI() {
            for (var index = 0; index < slots.Count; index++) {
                if (index < items.Count) {
                    slots[index].GetComponent<CanvasGroup>().DOFade(1, .25f);
                    
                    slotsImg[index].enabled = true;
                    slotsImg[index].sprite = items[index] switch {
                        Weapon w => w.sprite,
                        Armor a => a.sprite,
                        _ => slotsImg[index].sprite
                    };
                }
                else {
                    slots[index].GetComponent<CanvasGroup>().DOFade(.5f, .25f);
                    slotsImg[index].enabled = false;
                }
            }
        }
        
        
        
        /// <summary>
        /// Try to add an item to the inventory
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [ContextMenu("Add item")]
        private void TryAddItem() {
            if (items.Count >= slots.Count) return;
            items.Add(addItem);
            UpdateInventoryUI();
        }
        public ICatchable TryAddItem(ICatchable item, GameObject catchGam) {
            if (items.Count >= slots.Count) {
                catchGam.GetComponent<ThrowItem>().SetItem(items[currentSelectedSlot] as AbstractItem, null);
                items[currentSelectedSlot] = item;
                UpdateInventoryUI();
                
                return catchGam.GetComponent<ThrowItem>().Item;
            }
            
            items.Add(item);
            PlayerController.instance.ResetPressEText();
            Destroy(catchGam);
            UpdateInventoryUI();
            return null;
        }

        /// <summary>
        /// Remove the item from the inventory
        /// </summary>
        public void RemoveItem() {
            items.RemoveAt(currentSelectedSlot);
            UpdateInventoryUI();
        }

        /// <summary>
        /// Drop item
        /// </summary>
        /// <param name="forceDir"></param>
        /// <param name="itemA"></param>
        public void DropAbstractItem(Vector3 forceDir, Vector3 pos, GameObject notGrab, AbstractItem itemA) {
            Debug.Log("ok");
            GameObject item = Instantiate(itemGam, pos, Quaternion.identity);
            item.GetComponent<Rigidbody>().AddForce(forceDir, ForceMode.Impulse);
            item.GetComponent<ThrowItem>().SetItem(itemA, notGrab);
        }
    }
}