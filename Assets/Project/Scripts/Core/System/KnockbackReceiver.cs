using UnityEngine;

namespace Core.System
{
    public class KnockbackReceiver : MonoBehaviour
    {
        [SerializeField] private float knockbackResistance = 0f;
        [SerializeField] private float knockbackDuration = 0.15f;

        private Health health;
        private Mover mover;

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

        private void HandleDamaged(Vector2 hitDirection, float force)
        {
            // Resistance : 1 -> 완전 면역
            float actualForce = force * (1f - knockbackResistance);
            if (actualForce <= 0f) return;

            mover.ApplyKnockback(hitDirection, actualForce, knockbackDuration);
        }
    }
}