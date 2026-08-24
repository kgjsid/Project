using UnityEngine;

namespace Core.System
{
    public class PiercingProjectile : ProjectileBase
    {
        protected override void OnHitEnemy(Health health)
        {
            health.TakeDamage(damage, direction, knockbackForce);
        }
    }
}