using System;
using UnityEngine;

using Core.System.Pooling;
using Item.Data;

namespace Core.System
{
    public class ProjectileAttacker : MonoBehaviour, IAttacker
    {
        private ProjectileBase projectilePrefab;

        private float damage;
        private float projectileSpeed;
        private float knockbackForce;
        private float attackSpeed = 1f;
        private Sprite projectileSprite;
        private Color projectileColor;
        private int projectileCount = 1;
        private float spreadAngle = 0f;

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
            projectileSprite = weapon.projectileSprite;
            projectileColor = weapon.effectColor;
            projectileCount = weapon.projectileCount;
            spreadAngle = weapon.spreadAngle;
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
            lastAttackTime = Time.time;

            float startAngle = -spreadAngle * 0.5f;
            float step = projectileCount > 1 ? spreadAngle / (projectileCount - 1) : 0f;

            for(int i = 0; i < projectileCount; i++)
            {
                float angle = startAngle + step * i;
                Vector2 dir = Rotate(aimDirection, angle);
                FireProjectile(dir);
            }

            OnAttackPerformed?.Invoke();
        }

        private void FireProjectile(Vector2 dir)
        {
            IPoolable poolObj = PoolManager.Instance.Get(projectilePrefab.GetType());

            if (poolObj == null) return;
            if (poolObj is not ProjectileBase projectile) return;

            projectile.transform.position = transform.position;
            projectile.Launch(dir, projectileSpeed, damage, knockbackForce,
                              hitMask, obstacleMask, projectileSprite, projectileColor);
        }

        private Vector2 Rotate(Vector2 v, float degrees)
        {
            float rad = degrees * Mathf.Deg2Rad;
            float cos = Mathf.Cos(rad);
            float sin = Mathf.Sin(rad);
            return new Vector2(v.x * cos - v.y * sin, v.x * sin + v.y * cos);
        }
    }
}