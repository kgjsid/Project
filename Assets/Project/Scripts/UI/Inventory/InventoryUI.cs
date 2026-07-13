using UnityEngine;
using System.Collections.Generic;

using Core.System;
using UI.Inventory;

namespace Actors.UI
{
    public class InventoryUI : MonoBehaviour
    {
        private static InventoryUI instance;

        public Inventory targetInventory;       // UI가 보여줄 Inventory
        public SlotUI slotUIPrefab;
        public Transform slotParent;

        private List<SlotUI> uiSlots = new List<SlotUI>();

        public static InventoryUI Instance { get { return instance; } }

        private void Awake()
        {
            instance = this;
        }

        private void OnEnable()
        {
            if(targetInventory != null)
            {
                targetInventory.OnInventoryChanged += RefreshAll;
            }
        }

        private void Start()
        {
            gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            if(targetInventory != null)
            {
                targetInventory.OnInventoryChanged -= RefreshAll;
            }
        }

        public void SetTargetInventory(Inventory inventory)
        {
            // 이전 구독 해제 (기존에 연결된 인벤토리가 있었다면)
            if (targetInventory != null)
                targetInventory.OnInventoryChanged -= RefreshAll;

            targetInventory = inventory;

            // 새로운 인벤토리 구독 및 UI 초기화
            if (targetInventory != null)
            {
                targetInventory.OnInventoryChanged += RefreshAll;
                InitSlots(); // 인벤토리가 바뀌었으니 슬롯 개수도 다시 맞춤
            }
        }

        public void InitSlots()
        {
            foreach (Transform slot in slotParent) Destroy(slot.gameObject);
            uiSlots.Clear();

            for (int i = 0; i < targetInventory.Slots.Length; i++)
            {
                SlotUI newSlot = Instantiate(slotUIPrefab, slotParent);
                newSlot.slotIndex = i;
                newSlot.parentInventory = targetInventory;
                uiSlots.Add(newSlot);
            }

            RefreshAll();
        }

        public void RefreshAll()
        {
            for (int i = 0; i < uiSlots.Count; i++)
            {
                uiSlots[i].SetData(targetInventory.Slots[i]);
            }
        }

        public void Toggle()
        {
            bool isActive = !gameObject.activeSelf;
            gameObject.SetActive(isActive);

            if (isActive)
            {
                Open();
            }
            else
            {
                Close();
            }
        }

        private void Open()
        {
            gameObject.SetActive(true);
            RefreshAll();
        }

        private void Close()
        {
            gameObject.SetActive(false);

            if (LootUI.Instance != null && LootUI.Instance.gameObject.activeSelf)
                LootUI.Instance.Close();
        }
    }
}