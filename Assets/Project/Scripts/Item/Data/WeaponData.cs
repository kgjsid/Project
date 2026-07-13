using UnityEngine;

using Core.System;

namespace Item.Data
{
    [CreateAssetMenu(fileName = "NewWeapon", menuName = "Items/Weapon")]
    public class WeaponData : EquipmentData
    {
        public float damage;
        public float range;
        public float attackSpeed;
    }
}
