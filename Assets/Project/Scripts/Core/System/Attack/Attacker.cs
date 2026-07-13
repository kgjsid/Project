using Item.Data;
using UnityEngine;

namespace Core.System
{
    public class Attacker : MonoBehaviour
    {
        public LayerMask layerMask;

        private float damage;
        private float range;
        private float attackSpeed = 1f;
        private bool hasWeapon = false;

        private float lastAttackTime = -99f;

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

        public void Attack()
        {
            if (!hasWeapon) return;
            if (Time.time - lastAttackTime < 1f / attackSpeed) return;
            lastAttackTime = Time.time;

            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, range, layerMask))
            {
                if (hit.collider.TryGetComponent(out Health health))
                {
                    health.TakeDamage(damage);
                }
            }
        }
    }
}