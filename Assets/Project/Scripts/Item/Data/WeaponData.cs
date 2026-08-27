using UnityEngine;

using Core.System;

namespace Item.Data
{
    [System.Flags]
    public enum AttackType { None = 0, Melee = 1, Projectile = 2, Raycast = 4 }

    [System.Serializable]
    public class MeleeStats
    {
        public float damage;
        public float knockbackForce;
        public float attackSpeed;
        public float range;
        public float swingAngle;
        public Sprite telegraphSprite;
    }

    [System.Serializable]
    public class ProjectileStats
    {
        public float damage;
        public float knockbackForce;
        public float attackSpeed = 1f;
        public ProjectileBase projectilePrefab;
        public float projectileSpeed;
        public Sprite projectileSprite;
        public int projectileCount;
        public float spreadAngle;
    }

    [System.Serializable]
    public class RaycastStats
    {
        public float damage;
        public float knockbackForce;
        public float attackSpeed;
        public float range;
        public float boxWidth;
        public int beamCount;
        public float spreadAngle;
    }

    [CreateAssetMenu(fileName = "NewWeapon", menuName = "Items/Weapon")]
    public class WeaponData : EquipmentData
    {
        [Header("Attack Type (여럿 선택 시 하이브리드)")]
        public AttackType attackType;

        [Header("Common")]
        public Color effectColor = Color.white;
        public GameObject effectPrefab;

        [Header("Parts - attackType에 켠 것만 사용")]
        public MeleeStats melee;
        public ProjectileStats projectile;
        public RaycastStats raycast;
    }
}
