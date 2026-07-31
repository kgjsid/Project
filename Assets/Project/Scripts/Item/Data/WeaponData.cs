using UnityEngine;

using Core.System;

namespace Item.Data
{
    public enum AttackType { Melee, Ranged }

    [CreateAssetMenu(fileName = "NewWeapon", menuName = "Items/Weapon")]
    public class WeaponData : EquipmentData
    {
        public AttackType attackType;

        [Header("Weapon Stats")]
        public float damage;
        public float range;
        public float attackSpeed;
        public float knockbackForce;

        [Header("Effect")]
        public GameObject effectPrefab;
        public Color effectColor = Color.white;

        public Projectile projectilePrefab;
        public float projectileSpeed;
    }
}
