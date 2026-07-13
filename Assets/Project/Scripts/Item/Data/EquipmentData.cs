using UnityEngine;

namespace Item.Data
{
    [CreateAssetMenu(fileName = "NewEquip", menuName = "Items/Equipment")]
    public class EquipmentData : ItemData
    {
        public EquipSlotType equipSlot;
        public int durability;
    }
}