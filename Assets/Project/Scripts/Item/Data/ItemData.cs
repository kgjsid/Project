using UnityEngine;

namespace Item.Data
{
    /// <summary>
    /// 아이템 타입(재료, 장비, 소모품)
    /// </summary>
    public enum ItemType { Material, Equipment, Consumable }
    /// <summary>
    /// 장비 타입(헬멧, 아머, 첫번째 무기,..)
    /// </summary>
    public enum EquipSlotType { None, Helmet, Armor, PrimaryWeapon, SecondaryWeapon, Backpack}

    [CreateAssetMenu(fileName = "NewItem", menuName = "Item/ItemData")]
    public class ItemData : ScriptableObject
    {
        public string itemName;
        public ItemType itemType;
        public int maxStack = 999;
        public int sellPrice;
        public float weight = 0.1f;

        public Sprite icon;
        [TextArea] public string description;
    }
}