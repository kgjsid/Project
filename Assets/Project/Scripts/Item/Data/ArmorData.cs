using UnityEngine;

namespace Item.Data
{
    [CreateAssetMenu(fileName = "NewArmor", menuName = "Items/Equipment/Armor")]
    public class ArmorData : EquipmentData
    {
        public int defense;
        public float moveSpeedMod;
        public float weightBonus;
    }
}