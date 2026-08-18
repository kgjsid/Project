using System;
using UnityEngine;

using Item.Data;

namespace Core.System
{
    public class MeleeAttacker : MonoBehaviour, IAttacker
    {
        private LayerMask hitMask;
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

        private HitStop ownerHitStop;
        [SerializeField] private float hitStopDuration = 0.08f;

        private const int HIT_COLLIDER_COUNT = 10;
        private const float ROTATION_THRESHOLD = 0.0001f;

        public float SwingAngle { get { return swingAngle; } set { swingAngle = value; cosHalfSwing = Mathf.Cos(swingAngle * 0.5f * Mathf.Deg2Rad); } }

        public void SetWeapon(WeaponData weapon)
        {
            damage = weapon.damage;
            range = weapon.range;
            attackSpeed = weapon.attackSpeed;
            knockbackForce = weapon.knockbackForce;
            SwingAngle = weapon.swingAngle;
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

        public void SetHitStop(HitStop hitStop)
        {
            this.ownerHitStop = hitStop;
        }

        public void Attack()
        {
            if (hitColliders == null) hitColliders = new Collider2D[HIT_COLLIDER_COUNT];
            if (!hasWeapon) return;
            if (Time.time - lastAttackTime < 1f / attackSpeed) return;

            ExecuteAttack();
        }

        public void ForceAttack()
        {
            if (!hasWeapon) return;

            ExecuteAttack();
        }

        private void ExecuteAttack()
        {
            lastAttackTime = Time.time;

            if (hitColliders == null) hitColliders = new Collider2D[HIT_COLLIDER_COUNT];

            int size = Physics2D.OverlapCircle(transform.position, range, contactFilter, hitColliders);

            bool anyHit = false;

            for (int hitIndex = 0; hitIndex < size; hitIndex++)
            {
                Vector2 toTarget = ((Vector2)hitColliders[hitIndex].transform.position - (Vector2)transform.position).normalized;

                if (Vector2.Dot(toTarget, aimDirection) < cosHalfSwing) continue;

                if (hitColliders[hitIndex].TryGetComponent(out Health health))
                {
                    health.TakeDamage(damage, toTarget, knockbackForce);
                    anyHit = true;

                    if (hitColliders[hitIndex].TryGetComponent(out HitStop targetHitStop))
                    {
                        targetHitStop.Freeze(hitStopDuration);
                    }
                }
            }

            if (anyHit && ownerHitStop != null)
            {
                ownerHitStop.Freeze(hitStopDuration);
            }

            OnAttackPerformed?.Invoke();
        }
    }
}
