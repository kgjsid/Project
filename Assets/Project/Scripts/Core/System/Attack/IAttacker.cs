using System;
using UnityEngine;

using Item.Data;

namespace Core.System
{
    public interface IAttacker
    {
        event Action OnAttackPerformed;
        event Action<Vector2> OnAimDirectionChanged;

        void SetWeapon(WeaponData weapon);
        void ClearWeapon();
        void SetLayerMask(LayerMask targetMask);
        void SetAimDirection(Vector2 direction);
        void Attack();

        void ForceAttack();
    }
}