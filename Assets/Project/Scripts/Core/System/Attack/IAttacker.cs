using System;
using UnityEngine;

namespace Core.System
{
    public interface IAttacker
    {
        event Action OnAttackPerformed;
        event Action<Vector2> OnAimDirectionChanged;

        void SetAimDirection(Vector2 direction);
        void Attack();
        void ForceAttack();
    }
}