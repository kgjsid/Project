using System;
using UnityEngine;

using Item.Data;

namespace Core.System
{
    public class HybridAttacker : MonoBehaviour, IAttacker
    {
        private MeleeAttacker meleeAttacker;
        private ProjectileAttacker projectileAttacker;
        private RaycastAttacker raycastAttacker;

        private bool useMelee;
        private bool useProjectile;
        private bool useRaycast;

        public event Action OnAttackPerformed;
        public event Action<Vector2> OnAimDirectionChanged;

        public void Init(MeleeAttacker melee, ProjectileAttacker projectile, RaycastAttacker raycast)
        {
            meleeAttacker = melee;
            projectileAttacker = projectile;
            raycastAttacker = raycast;
        }

        public void SetWeapon(WeaponData weapon)
        {
            useMelee = weapon.attackType.HasFlag(AttackType.Melee);
            useProjectile = weapon.attackType.HasFlag(AttackType.Projectile);
            useRaycast = weapon.attackType.HasFlag(AttackType.Raycast);

            if (useMelee) meleeAttacker.SetWeapon(weapon);
            else meleeAttacker.ClearWeapon();

            if (useProjectile) projectileAttacker.SetWeapon(weapon);
            else projectileAttacker.ClearWeapon();

            if (useRaycast) raycastAttacker.SetWeapon(weapon);
            else raycastAttacker.ClearWeapon();
        }

        public void ClearWeapon()
        {
            useMelee = false;
            useProjectile = false;
            useRaycast = false;

            meleeAttacker.ClearWeapon();
            projectileAttacker.ClearWeapon();
            raycastAttacker.ClearWeapon();
        }

        public void SetAimDirection(Vector2 direction)
        {
            if (useMelee) meleeAttacker.SetAimDirection(direction);
            if (useProjectile) projectileAttacker.SetAimDirection(direction);
            if (useRaycast) raycastAttacker.SetAimDirection(direction);

            OnAimDirectionChanged?.Invoke(direction);
        }

        public void Attack()
        {
            if (useMelee) meleeAttacker.Attack();
            if (useProjectile) projectileAttacker.Attack();
            if (useRaycast) raycastAttacker.Attack();

            OnAttackPerformed?.Invoke();
        }

        public void ForceAttack()
        {
            if (useMelee) meleeAttacker.ForceAttack();
            if (useProjectile) projectileAttacker.ForceAttack();
            if (useRaycast) raycastAttacker.ForceAttack();

            OnAttackPerformed?.Invoke();
        }
    }
}