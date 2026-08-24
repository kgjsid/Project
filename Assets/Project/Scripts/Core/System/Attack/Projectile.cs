namespace Core.System
{
    public class Projectile : ProjectileBase
    {
        protected override void OnHitEnemy(Health health)
        {
            health.TakeDamage(damage, direction, knockbackForce);
            Despawn();
        }
    }
}