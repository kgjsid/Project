using UnityEngine;

namespace Item.Data
{
    [CreateAssetMenu(fileName = "NewArmor", menuName = "Items/Armor")]
    public class ArmorData : EquipmentData
    {
        public int defense;
        public float moveSpeedMod;
        public float weightBonus;
        public float maxHpBonus;
        [Range(0f, 1f)] public float knockbackResistBonus;
    }
}