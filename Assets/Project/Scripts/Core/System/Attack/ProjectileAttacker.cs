using Item.Data;
using System;
using UnityEngine;

namespace Core.System
{
    public class ProjectileAttacker : MonoBehaviour, IAttacker
    {
        public Projectile projectilePrefab;
        public LayerMask hitMask;

        private float damage;
        private float projectileSpeed;
        private float attackSpeed = 1f;
        private bool hasWeapon;
        private float lastAttackTime = -1f;
        private Vector2 aimDirection = Vector2.right;

        public event Action OnAttackPerformed;
        public event Action<Vector2> OnAimDirectionChanged;

        private const float ROTATION_THRESHOLD = 0.0001f;

        public void SetWeapon(WeaponData weapon)
        {
            damage = weapon.damage;
            projectileSpeed = weapon.projectileSpeed;
            attackSpeed = weapon.attackSpeed;
            hasWeapon = true;
        }

        public void ClearWeapon()
        {
            hasWeapon = false;
        }

        public void SetLayerMask(LayerMask targetMask)
        {
            hitMask = targetMask;
        }

        public void SetAimDirection(Vector2 direction)
        {
            if (direction.sqrMagnitude < ROTATION_THRESHOLD) return;
            aimDirection = direction.normalized;
            OnAimDirectionChanged?.Invoke(aimDirection);
        }

        public void Attack()
        {
            if (!hasWeapon || projectilePrefab == null) return;
            if (Time.time - lastAttackTime < 1f / attackSpeed) return;
            lastAttackTime = Time.time;

            Projectile projectileObj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            projectileObj.Launch(aimDirection, projectileSpeed, damage, hitMask);

            OnAttackPerformed?.Invoke();
        }
    }
}