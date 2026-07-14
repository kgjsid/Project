using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

using Core.System;
using Item.Data;

namespace UI.Inventory
{
    /// <summary>
    /// 인벤토리/장비창의 슬롯 한 칸을 담당하는 UI.
    /// 표시(ItemSlot) 및 드래그드랍 기능 포함
    /// </summary>
    public class SlotUI : MonoBehaviour, 
        IPointerClickHandler,
        IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
    {
        public int slotIndex;                           // 슬롯 번호(몇 번 슬롯인지)
        public Core.System.Inventory parentInventory;   // 슬롯이 속한 인벤토리(Player, Inventory..)
        public EquipSlotType slotType;                  // None : 일반 슬롯. 그 외에는 장비 슬롯(무기, 갑옷 등)

        public Image itemIcon;                          // 슬롯에 해당하는 아이템 아이콘
        public TextMeshProUGUI countText;

        private Color normalColor = new Color(1f, 1f, 1f, 1f);
        private Color dragColor = new Color(1f, 1f, 1f, 0.5f);

        public void SetData(ItemSlot slot)
        {
            if(slot == null || slot.IsEmpty())
            {
                itemIcon.gameObject.SetActive(false);
                countText.text = string.Empty;

                return;
            }

            itemIcon.gameObject.SetActive(true);
            itemIcon.sprite = slot.item.icon != null? slot.item.icon : null;
            countText.text = slot.count > 1 ? slot.count.ToString() : string.Empty;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.clickCount != 2) return;

            ItemData item = parentInventory.Slots[slotIndex].item;
            if(item is ConsumableData consumable)
            {
                consumable.Use(parentInventory.gameObject);
                parentInventory.ConsumeItem(slotIndex);
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (parentInventory.Slots[slotIndex].IsEmpty()) return;

            DragManager.Instance.StartDrag(this);
            itemIcon.color = dragColor;
        }

        public void OnDrag(PointerEventData eventData)
        {
            DragManager.Instance.UpdateDrag();
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            DragManager.Instance.EndDrag();
            itemIcon.color = normalColor;
        }

        /// <summary>
        /// 드롭 시(이동 및 장비 장착 해제)
        /// </summary>
        public void OnDrop(PointerEventData eventData)
        {
            SlotUI fromSlot = DragManager.Instance.currentDragSlotUI;
            
            // 자기 자신 슬롯은 무시
            if (fromSlot == null || fromSlot == this) return;

            ItemData draggedItem = fromSlot.parentInventory.Slots[fromSlot.slotIndex].item; // 드래그 아이템
            ItemData targetItem = this.parentInventory.Slots[this.slotIndex].item;          // 타겟(드래그 지점에 있던 아이템, 교체 대상)

            if (!IsValidDrop(fromSlot, draggedItem, targetItem)) return;

            fromSlot.parentInventory.Swap(fromSlot.slotIndex, this.slotIndex, this.parentInventory);

            SyncEquipmentSwap(fromSlot, draggedItem);
        }

        private bool IsValidDrop(SlotUI fromSlot, ItemData draggedItem, ItemData targetItem)
        {
            // 현재 UI slotType 체크 -> 장비 or 기본칸(인벤토리)
            // 대상이 장비 슬롯인 경우 -> 해당 슬롯에 맞는 장비 아이템(helmet - helmet..)만 드래그 가능
            
            // 대상 슬롯 타입이 장비칸인 경우(장비를 장착하는 경우)
            if (slotType != EquipSlotType.None)
            {   
                // 드래그 된 아이템이 장비 아이템이 아니거나, 슬롯 타입과 일치하지 않다면 드랍 불가
                if (!(draggedItem is EquipmentData e) || e.equipSlot != slotType) return false;
            }

            // 출발지가 장비 슬롯, 타겟에 이미 다른 아이템이 있는 경우
            // ex) 장비된 helmet과 인벤토리의 helmet이 변경되는 경우
            if (fromSlot.slotType != EquipSlotType.None && targetItem != null)
            {   
                // 드래그 된 아이템이 장비 아이템이 아니거나 슬롯 타입과 일치하지 않다면 드랍 불가
                if (!(targetItem is EquipmentData e) || e.equipSlot != fromSlot.slotType) return false;
            }

            return true;
        }

        /// <summary>
        /// Equipper에 데이터 반영(장착, 해제)
        /// </summary>
        private void SyncEquipmentSwap(SlotUI fromSlot, ItemData draggedItem)
        {
            if (slotType != EquipSlotType.None && draggedItem is EquipmentData equipData)
            {
                this.parentInventory.GetComponent<Equipper>()?.Equip(equipData);
            }

            if (fromSlot.slotType != EquipSlotType.None)
            {
                fromSlot.parentInventory.GetComponent<Equipper>()?.Unequip(fromSlot.slotType);
            }
        }
    }
}