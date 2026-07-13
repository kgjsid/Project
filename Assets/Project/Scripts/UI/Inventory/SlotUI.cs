using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

using Actors.UI;
using Item.Data;

namespace UI.Inventory
{
    public class SlotUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
    {
        public int slotIndex;                           // 슬롯 번호
        public Core.System.Inventory parentInventory;   // 슬롯이 속한 인벤토리(Player, Inventory..)
        public EquipSlotType slotType;

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
            }
            else
            {
                itemIcon.gameObject.SetActive(true);
                itemIcon.sprite = slot.item.icon != null? slot.item.icon : null;
                countText.text = slot.count > 1 ? slot.count.ToString() : string.Empty;
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

        public void OnDrop(PointerEventData eventData)
        {
            SlotUI fromSlot = DragManager.Instance.currentDragSlotUI;
            
            if (fromSlot == null || fromSlot == this) return;

            ItemData draggedItem = fromSlot.parentInventory.Slots[fromSlot.slotIndex].item; // 드래그 아이템
            ItemData targetItem = this.parentInventory.Slots[this.slotIndex].item;          // 타겟(드래그 지점에 있던 아이템)

            // 1. 내가 장비창일 때
            // -> 들어오는 템 체크
            if (slotType != EquipSlotType.None)
            {
                if (!(draggedItem is EquipmentData e) || e.equipSlot != slotType) return;
            }

            // 2. 상대가 장비창일 때
            // -> 내가 보내는 템 체크 (역방향)
            if (fromSlot.slotType != EquipSlotType.None)
            {
                // 내 슬롯이 비어있지 않다면, 저쪽(장비창)으로 가도 되는 템인지 확인
                if (targetItem != null)
                {
                    if (!(targetItem is EquipmentData e) || e.equipSlot != fromSlot.slotType) return;
                }
            }

            fromSlot.parentInventory.Swap(fromSlot.slotIndex, this.slotIndex, this.parentInventory);
        }
    }
}