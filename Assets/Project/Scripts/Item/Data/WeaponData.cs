using UnityEngine;

using Core.System;

namespace Item.Data
{
    public enum AttackType { Melee, Ranged }

    [CreateAssetMenu(fileName = "NewWeapon", menuName = "Items/Weapon")]
    public class WeaponData : EquipmentData
    {
        public AttackType attackType;

        public float damage;
        public float range;
        public float attackSpeed;

        public Projectile projectilePrefab;
        public float projectileSpeed;
    }
}
