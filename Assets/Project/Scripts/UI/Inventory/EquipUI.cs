using UnityEngine;

namespace UI.Inventory
{
    /// <summary>
    /// 장비창 UI
    /// </summary>
    public class EquipUI : MonoBehaviour
    {
        private static EquipUI instance;
        public static EquipUI Instance { get { return instance; } }

        public Core.System.Inventory targetInventory;
        public SlotUI[] equipSlots;

        private void Awake()
        {
            instance = this;
        }

        private void OnEnable()
        {
            if (targetInventory != null)
            {
                targetInventory.OnInventoryChanged += RefreshAll;
            }
        }

        private void OnDisable()
        {
            if (targetInventory != null)
            {
                targetInventory.OnInventoryChanged -= RefreshAll;
            }
        }

        public void SetTargetInventory(Core.System.Inventory inventory)
        {
            if (targetInventory != null)
            {
                targetInventory.OnInventoryChanged -= RefreshAll;
            }

            targetInventory = inventory;

            if (targetInventory != null)
            {
                targetInventory.OnInventoryChanged += RefreshAll;
                
                // 할당해둔 슬롯 미리 활용(장비 슬롯)
                for (int i = 0; i < equipSlots.Length; i++)
                {
                    equipSlots[i].slotIndex = i;
                    equipSlots[i].parentInventory = targetInventory;
                }

                RefreshAll();
            }
        }

        public void RefreshAll()
        {
            if (targetInventory == null) return;
            for (int i = 0; i < equipSlots.Length; i++)
            {
                if (equipSlots[i] == null) continue;
                equipSlots[i].SetData(targetInventory.Slots[i]);
            }
        }
    }
}