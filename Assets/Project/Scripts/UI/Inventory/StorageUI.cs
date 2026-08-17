using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Inventory
{
    public class StorageUI : MonoBehaviour
    {
        private static StorageUI instance;

        private Core.System.Inventory stashInventory;
        public SlotUI slotUIPrefab;
        public Transform slotParent;
        public Button closeButton;

        public Dictionary<int, List<SlotUI>> itemDictionary = new Dictionary<int, List<SlotUI>>();
        public List<SlotUI> uiSlots = new List<SlotUI>();

        private bool isInitialized;

        public static StorageUI Instance { get { return instance; } }

        private void Awake()
        {
            instance = this;

            closeButton?.onClick.AddListener(Close);
            gameObject.SetActive(false);
        }

        public void Open(Core.System.Inventory stash)
        {
            if(stashInventory != null)
            {
                stashInventory.OnInventoryChanged -= RefreshAll;
            }

            stashInventory = stash;
            stashInventory.OnInventoryChanged += RefreshAll;

            gameObject.SetActive(true);

            if(!isInitialized)
            {
                InitSlot();
                isInitialized = true;
            }
            else
            {
                RefreshAll();
            }

            if (InventoryUI.Instance != null && !InventoryUI.Instance.gameObject.activeSelf)
            {
                InventoryUI.Instance.Toggle();
            }
        }

        public void Close()
        {
            if (stashInventory != null)
            {
                stashInventory.OnInventoryChanged -= RefreshAll;
            }

            gameObject.SetActive(false);

            if (InventoryUI.Instance != null && InventoryUI.Instance.gameObject.activeSelf)
            {
                InventoryUI.Instance.Toggle();
            }
        }

        public void RefreshAll()
        {
            if (stashInventory == null) return;

            foreach (var slot in uiSlots)
            {
                if (slot == null) continue;
                slot.SetData(stashInventory.Slots[slot.slotIndex]);
            }
        }

        private void InitSlot()
        {
            foreach (Transform child in slotParent) Destroy(child.gameObject);
            uiSlots.Clear();

            for (int i = 0; i < stashInventory.Slots.Length; i++)
            {
                var slot = Instantiate(slotUIPrefab, slotParent);
                slot.slotIndex = i;
                slot.parentInventory = stashInventory;

                uiSlots.Add(slot);
            }
            RefreshAll();
        }
    }
}