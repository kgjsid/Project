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
            attackSpeed = weapon.attackSpeed;
            projectileSpeed = weapon.projectileSpeed;
            projectilePrefab = weapon.projectilePrefab;
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
            
            ExecuteAttack();
        }

        public void ForceAttack()
        {
            if (!hasWeapon || projectilePrefab == null) return;

            ExecuteAttack();
        }

        private void ExecuteAttack()
        {
            lastAttackTime = Time.time;

            Projectile projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            projectile.Launch(aimDirection, projectileSpeed, damage, hitMask);

            OnAttackPerformed?.Invoke();
        }
    }
}