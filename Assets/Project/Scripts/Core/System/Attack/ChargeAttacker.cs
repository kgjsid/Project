using System;
using System.Collections.Generic;
using UnityEngine;

using Item.Data;

namespace Core.System
{
    public class ChargeAttacker : MonoBehaviour, IAttacker
    {
        private Mover mover;

        private float damage;
        private float knockbackForce;
        private float attackSpeed;
        private float dashSpeed;
        private float dashDuration;
        private float hitRadius;

        private bool hasWeapon;
        private float lastAttackTime = -1f;
        private Vector2 aimDirection = Vector2.right;

        private bool isCharging;
        private float chargeTimer;
        private Vector2 chargeDirection;

        private LayerMask hitMask;
        private ContactFilter2D hitFilter;
        private Collider2D[] hitResults;
        private HashSet<Health> alreadyHit;

        public event Action OnAttackPerformed;
        public event Action<Vector2> OnAimDirectionChanged;

        private const int HIT_RESULT_COUNT = 16;
        private const float ROTATION_THRESHOLD = 0.00001f;

        private void Awake()
        {
            hitResults = new Collider2D[HIT_RESULT_COUNT];
            alreadyHit = new HashSet<Health>();
        }

        public void Init(Mover mover)
        {
            this.mover = mover;
        }

        public void SetWeapon(WeaponData weapon)
        {
            damage = weapon.charge.damage;
            knockbackForce = weapon.charge.knockbackForce;
            attackSpeed = weapon.charge.attackSpeed;
            dashSpeed = weapon.charge.dashSpeed;
            dashDuration = weapon.charge.dashDuration;
            hitRadius = weapon.charge.hitRadius;
            hasWeapon = true;
        }

        public void ClearWeapon()
        {
            hasWeapon = false;
        }

        public void SetLayerMask(LayerMask hitMask)
        {
            this.hitMask = hitMask;
            hitFilter = new ContactFilter2D();
            hitFilter.SetLayerMask(hitMask);
            hitFilter.useTriggers = true;
        }

        public void SetAimDirection(Vector2 direction)
        {
            if (direction.sqrMagnitude < ROTATION_THRESHOLD) return;
            aimDirection = direction.normalized;
            OnAimDirectionChanged?.Invoke(aimDirection);
        }

        public void Attack()
        {
            if (!hasWeapon || isCharging) return;
            if (Time.time - lastAttackTime < 1f / attackSpeed) return;
            StartCharge();
        }

        public void ForceAttack()
        {
            if (!hasWeapon || isCharging) return;
            StartCharge();
        }

        private void StartCharge()
        {
            lastAttackTime = Time.time;

            isCharging = true;
            chargeTimer = dashDuration;
            chargeDirection = aimDirection;
            alreadyHit.Clear();

            mover.ApplyDash(chargeDirection, dashSpeed, dashDuration);

            OnAttackPerformed?.Invoke();
        }

        private void Update()
        {
            if (!isCharging) return;

            int count = Physics2D.OverlapCircle(transform.position, hitRadius, hitFilter, hitResults);

            for(int i = 0; i < count; i++)
            {
                if (hitResults[i].TryGetComponent(out Health health))
                {
                    if (alreadyHit.Contains(health)) continue;
                    alreadyHit.Add(health);
                    health.TakeDamage(damage, chargeDirection, knockbackForce);
                }
            }

            chargeTimer -= Time.deltaTime;
            if (chargeTimer <= 0f)
            {
                isCharging = false;
            }
        }
    }
}