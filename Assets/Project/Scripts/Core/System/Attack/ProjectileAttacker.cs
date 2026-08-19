using System;
using UnityEngine;

using Core.System.Pooling;
using Item.Data;

namespace Core.System
{
    public class ProjectileAttacker : MonoBehaviour, IAttacker
    {
        private Projectile projectilePrefab;

        private float damage;
        private float projectileSpeed;
        private float knockbackForce;
        private float attackSpeed = 1f;
        private bool hasWeapon;
        private float lastAttackTime = -1f;
        private Vector2 aimDirection = Vector2.right;

        private LayerMask hitMask;
        private LayerMask obstacleMask;

        public event Action OnAttackPerformed;
        public event Action<Vector2> OnAimDirectionChanged;

        private const float ROTATION_THRESHOLD = 0.00001f;

        public void SetWeapon(WeaponData weapon)
        {
            damage = weapon.damage;
            attackSpeed = weapon.attackSpeed;
            projectileSpeed = weapon.projectileSpeed;
            knockbackForce = weapon.knockbackForce;
            projectilePrefab = weapon.projectilePrefab;
            hasWeapon = true;
        }

        public void ClearWeapon()
        {
            hasWeapon = false;
        }

        public void SetLayerMask(LayerMask hitMask)
        {
            this.hitMask = hitMask;
        }

        public void SetObstacleMask(LayerMask obstacleMask)
        {
            this.obstacleMask = obstacleMask;
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
            Projectile projectile = PoolManager.Instance.Get<Projectile>();

            if (projectile == null) return;

            lastAttackTime = Time.time;

            projectile.transform.position = transform.position;
            projectile.Launch(aimDirection, projectileSpeed, damage, knockbackForce, hitMask, obstacleMask);

            OnAttackPerformed?.Invoke();
        }
    }
}