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

        public Sprite telegraphSprite;
        public float swingAngle;

        [Header("Projectile")]
        public ProjectileBase projectilePrefab;
        public float projectileSpeed;
        public Sprite projectileSprite;
        public int projectileCount = 1;
        public float spreadAngle = 0f;
    }
}
