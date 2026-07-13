using UnityEngine;
using System.Collections.Generic;

using UI.Inventory;
using DebugTester;
using UnityEngine.UI;
using Core.System;

namespace Actors.UI
{
    /// <summary>
    /// Looting 관련 UI를 관리할 싱글톤
    /// </summary>
    public class LootUI : MonoBehaviour
    {
        private static LootUI instance;

        public Inventory currentBoxInventory;
        public SlotUI slotUIPrefab;
        public Transform slotParent;
        public Button closeButton;

        private List<SlotUI> uiSlots = new List<SlotUI>();

        public static LootUI Instance { get { return instance; } }

        private void Awake()
        {
            instance = this;

            closeButton?.onClick.AddListener(Close);
            gameObject.SetActive(false);
        }

        public void Open(Inventory boxInventory)
        {
            // 1. 기존 상자와의 이벤트 연결 해제 (안전장치)
            if (currentBoxInventory != null)
                currentBoxInventory.OnInventoryChanged -= RefreshAll;

            // 2. 새 상자 할당 및 이벤트 구독
            currentBoxInventory = boxInventory;
            currentBoxInventory.OnInventoryChanged += RefreshAll;

            gameObject.SetActive(true);

            // 3. 슬롯 생성 및 초기화
            InitSlot();

            // 4. 플레이어 인벤토리도 같이 열어줌 (파밍 편의성)
            if (InventoryUI.Instance != null && !InventoryUI.Instance.gameObject.activeSelf)
            {
                InventoryUI.Instance.Toggle();
            }

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        public void Close()
        {
            // 닫을 때 이벤트 구독 해제 (메모리 누수 및 오작동 방지)
            if (currentBoxInventory != null)
            {
                currentBoxInventory.OnInventoryChanged -= RefreshAll;
                currentBoxInventory = null;
            }

            gameObject.SetActive(false);

            // 루트박스를 닫을 때 인벤토리 창만 남겨둘지, 같이 닫을지 결정
            // 보통 익스트랙션 게임은 상자를 닫으면 인벤토리도 같이 닫히는 경우가 많습니다.
            if (InventoryUI.Instance != null && InventoryUI.Instance.gameObject.activeSelf)
                InventoryUI.Instance.Toggle();
        }

        public void RefreshAll()
        {
            if (currentBoxInventory == null) return;

            foreach (var slot in uiSlots)
            {
                slot.SetData(currentBoxInventory.Slots[slot.slotIndex]);
            }
        }

        private void InitSlot()
        {
            foreach (Transform child in slotParent) Destroy(child.gameObject);
            uiSlots.Clear();

            for (int i = 0; i < currentBoxInventory.Slots.Length; i++)
            {
                var slot = Instantiate(slotUIPrefab, slotParent);
                slot.slotIndex = i;
                slot.parentInventory = currentBoxInventory;

                uiSlots.Add(slot);
            }
            RefreshAll();
        }
    }
}
