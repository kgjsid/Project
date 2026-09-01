using UnityEngine;

using Core.System;

namespace Item.Data
{
    [System.Flags]
    public enum AttackType { None = 0, Melee = 1, Projectile = 2, Raycast = 4, Charge = 8, }

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
        public Sprite telegraphSprite;
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

    [System.Serializable]
    public class ChargeStats
    {
        public float damage;
        public float knockbackForce;
        public float attackSpeed;
        public float dashSpeed;
        public float dashDuration;
        public float hitRadius;
        public Sprite telegraphSprite;
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
        public ChargeStats charge;

        private const float RANGED_TELEGRAPH_LENGTH = 5f;

        public Sprite GetTelegraphSprite()
        {
            if (attackType.HasFlag(AttackType.Melee) && melee.telegraphSprite != null)
                return melee.telegraphSprite;
            if (attackType.HasFlag(AttackType.Projectile) && projectile.telegraphSprite != null)
                return projectile.telegraphSprite;
            if (attackType.HasFlag(AttackType.Charge) && charge.telegraphSprite != null)
                return charge.telegraphSprite;
            return null;
        }

        public float GetTelegraphRange()
        {
            if (attackType.HasFlag(AttackType.Melee))
                return melee.range;
            if (attackType.HasFlag(AttackType.Raycast))
                return raycast.range;
            if (attackType.HasFlag(AttackType.Projectile))
                return RANGED_TELEGRAPH_LENGTH;
            if (attackType.HasFlag(AttackType.Charge))
                return charge.dashDuration * charge.dashDuration;
            return 0f;
        }
    }
}
