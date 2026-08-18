using UnityEngine;

namespace Core.System
{
    public class KnockbackReceiver : MonoBehaviour
    {
        [SerializeField] private float knockbackDuration = 0.15f;

        private Health health;
        private Mover mover;

        private bool isImmune;
        private float knockbackResistance;

        public float KnockbackResistance { get { return knockbackResistance; } set { knockbackResistance = value; } }
        public bool IsImmune { get { return isImmune; } set { isImmune = value; } }

        public void Init(Health health, Mover mover)
        {
            this.health = health;
            this.mover = mover;

            health.OnDamaged += HandleDamaged;
        }

        private void OnDisable()
        {
            if (health != null) health.OnDamaged -= HandleDamaged;
        }

        private void HandleDamaged(float damage,Vector2 hitDirection, float force)
        {
            // 경직 면역 상태
            if (IsImmune) return;

            // Resistance : 1 -> 완전 면역
            float actualForce = force * (1f - knockbackResistance);
            if (actualForce <= 0f) return;

            mover.ApplyKnockback(hitDirection, actualForce, knockbackDuration);
        }
    }
}