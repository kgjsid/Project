using System;
using UnityEngine;

using Item.Data;
using Item.ItemObject;
using DebugTester;

namespace Core.System
{
    public class Inventory : MonoBehaviour
    {
        private ItemSlot[] slots;

        public event Action OnInventoryChanged;

        private int capacity;

        public int Capacity { get { return capacity; } }
        public ItemSlot[] Slots { get { return slots; } }

        public void InitSlot(int capacity)
        {
            this.capacity = capacity;
            slots = new ItemSlot[capacity];

            for (int i = 0; i < Slots.Length; i++)
            {
                Slots[i] = new ItemSlot();
            }
            NotifyChange();
        }

        public bool AddItem(ItemData item, int count = 1)
        {
            // 1. 해당 아이템이 있는 경우(슬롯에서 찾아서 stack 추가)
            for(int i = 0; i < Slots.Length; i++)
            {
                if (!Slots[i].IsEmpty() && Slots[i].item == item && Slots[i].count < item.maxStack)
                {
                    Slots[i].count += count;
                    NotifyChange();
                    return true;
                }
            }

            // 2. 없으면 빈 슬롯에 새로 배치
            for (int i = 0; i < Slots.Length; i++)
            {
                if (Slots[i].IsEmpty())
                {
                    Slots[i] = new ItemSlot(item, count);
                    NotifyChange();
                    return true;
                }
            }

            return false;
        }

        public void Swap(int fromIndex, int toIndex, Inventory targetInventory = null)
        {   // target이 없는 경우 하나의 인벤토리 내부에서의 교체
            Inventory target = targetInventory ?? this;

            if (fromIndex < 0 || fromIndex >= Slots.Length) return;
            if (toIndex < 0 || toIndex >= target.Slots.Length) return;

            ItemSlot temp = this.Slots[fromIndex];
            this.Slots[fromIndex] = target.Slots[toIndex];
            target.Slots[toIndex] = temp;

            NotifyChange();
            if (target != this) target.NotifyChange();
        }

        /// <summary>
        /// 호출한 인벤토리의 모든 아이템을 다른 인벤토리로 옮기는 메소드
        /// </summary>
        /// <param name="target"></param>
        public void MoveItemsTo(Inventory target)
        {
            if (target == null) return;

            // 슬롯 중 아이템을 가지고 있는 것을 반복하면서 체크
            // 후에 target에게 해당 아이템을 넘겨줌.
            for(int i = 0; i < Slots.Length; i++)
            {
                if (Slots[i].IsEmpty()) continue;

                bool moved = target.AddItem(Slots[i].item, Slots[i].count);
                if (moved) Slots[i].Clear();
            }

            NotifyChange();
        }

        /// <summary>
        /// Inventory에 아이템이 있는지 검사
        /// </summary>
        /// <returns>하나라도 있다면 true</returns>
        public bool HasAnyItem()
        {             
            foreach(var slot in Slots)
            {
                if (!slot.IsEmpty()) return true;
            }

            return false;
        }

        /// <summary>
        /// LootBox로 변환.
        /// Inventory의 아이템을 LootBox로 옮기며, 프리팹 생성
        /// </summary>
        /// <param name="lootBoxPrefab"></param>
        public void DropAsLootBox(LootBox lootBoxPrefab)
        {
            // 아무 아이템도 없었다면 LootBox 생성을 하지 않음
            if (!HasAnyItem()) return;

            LootBox box = Instantiate(lootBoxPrefab, transform.position, Quaternion.identity);

            MoveItemsTo(box.Inventory);
        }

        /// <summary>
        /// 아이템 소모용 메소드
        /// </summary>
        /// <param name="index">슬롯 인덱스</param>
        public void ConsumeItem(int index)
        {
            if (index < 0 || index >= Slots.Length) return;
            if (Slots[index].IsEmpty()) return;

            Slots[index].count--;
            if (Slots[index].count <= 0) Slots[index].Clear();

            NotifyChange();
        }

        public void NotifyChange()
        {
            OnInventoryChanged?.Invoke();
        }
    }
}