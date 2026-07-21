using System;
using UnityEngine;

using Item.Data;

namespace Core.System
{
    public class MeleeAttacker : MonoBehaviour, IAttacker
    {
        public LayerMask hitMask;
        [SerializeField] private float hitRadius = 0.6f;

        private float damage;
        private float range;
        private float attackSpeed = 1f;
        private bool hasWeapon;
        private float lastAttackTime = -1f;
        private Vector2 aimDirection = Vector2.right;

        public event Action OnAttackPerformed;
        public event Action<Vector2> OnAimDirectionChanged;

        private ContactFilter2D contactFilter;
        private Collider2D[] hitColliders;

        private const int HIT_COLLIDER_COUNT = 10;
        private const float ROTATION_THRESHOLD = 0.0001f;

        public void SetWeapon(WeaponData weapon)
        {
            damage = weapon.damage;
            range = weapon.range;
            attackSpeed = weapon.attackSpeed;
            hasWeapon = true;
        }

        public void ClearWeapon()
        {
            hasWeapon = false;
        }

        public void SetAimDirection(Vector2 direction)
        {
            if (direction.sqrMagnitude < ROTATION_THRESHOLD) return;
            aimDirection = direction.normalized;
            OnAimDirectionChanged?.Invoke(aimDirection);
        }

        public void SetLayerMask(LayerMask targetMask)
        {
            hitMask = targetMask;

            contactFilter = new ContactFilter2D();
            contactFilter.SetLayerMask(hitMask);
            contactFilter.useTriggers = true;
        }

        public void Attack()
        {
            if (hitColliders == null) hitColliders = new Collider2D[HIT_COLLIDER_COUNT];
            if (!hasWeapon) return;
            if (Time.time - lastAttackTime < 1f / attackSpeed) return;
            lastAttackTime = Time.time;

            Vector2 hitPoint = (Vector2)transform.position + aimDirection * range;

            int size = Physics2D.OverlapCircle(hitPoint, hitRadius, contactFilter, hitColliders);
            for (int hitIndex = 0; hitIndex < size; hitIndex++)
            {
                if (hitColliders[hitIndex].TryGetComponent(out Health health))
                {
                    health.TakeDamage(damage);
                }
            }

            OnAttackPerformed?.Invoke();
        }

    }
}
