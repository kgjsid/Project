using System;
using UnityEngine;

using Item.Data;

namespace Core.System
{
    public class MeleeAttacker : MonoBehaviour, IAttacker
    {
        public LayerMask hitMask;
        [SerializeField] private float swingAngle = 100f;
        
        private float damage;
        private float range;
        private float attackSpeed = 1f;
        private float knockbackForce;
        private bool hasWeapon;
        private float lastAttackTime = -1f;
        private Vector2 aimDirection = Vector2.right;

        private float cosHalfSwing;

        public event Action OnAttackPerformed;
        public event Action<Vector2> OnAimDirectionChanged;

        private ContactFilter2D contactFilter;
        private Collider2D[] hitColliders;

        private const int HIT_COLLIDER_COUNT = 10;
        private const float ROTATION_THRESHOLD = 0.0001f;

        public float SwingAngle { get { return swingAngle; } set { swingAngle = value; cosHalfSwing = Mathf.Cos(swingAngle * 0.5f * Mathf.Deg2Rad); } }

        private void Awake()
        {
            cosHalfSwing = Mathf.Cos(swingAngle * 0.5f * Mathf.Deg2Rad);
        }

        public void SetWeapon(WeaponData weapon)
        {
            damage = weapon.damage;
            range = weapon.range;
            attackSpeed = weapon.attackSpeed;
            knockbackForce = weapon.knockbackForce;
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

            int size = Physics2D.OverlapCircle(transform.position, range, contactFilter, hitColliders);

            for (int i = 0; i < size; i++)
            {
                Vector2 toTarget = ((Vector2)hitColliders[i].transform.position - (Vector2)transform.position).normalized;

                if (Vector2.Dot(toTarget, aimDirection) < cosHalfSwing) continue;

                if (hitColliders[i].TryGetComponent(out Health health))
                {
                    health.TakeDamage(damage, toTarget, knockbackForce);
                }
            }

            OnAttackPerformed?.Invoke();
        }

    }
}
