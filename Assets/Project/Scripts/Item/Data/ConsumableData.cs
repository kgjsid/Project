using UnityEngine;

namespace Item.Data
{
    public enum ConsumableEffectType { Heal }

    [CreateAssetMenu(fileName = "NewConsumable", menuName = "Items/Consumable")]
    public class ConsumableData : ItemData
    {
        public ConsumableEffectType effectType;
        public float effectAmount;

        public void Use(GameObject user)
        {
            switch(effectType)
            {
                case ConsumableEffectType.Heal:
                    if (user.TryGetComponent(out Core.System.Health health))
                    {
                        health.Heal(effectAmount);
                    }
                    break;
            }
        }
    }
}
