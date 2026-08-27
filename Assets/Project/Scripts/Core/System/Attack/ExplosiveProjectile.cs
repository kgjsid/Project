using UnityEngine;

namespace Core.System
{
    public class ExplosiveProjectile : ProjectileBase
    {
        private Collider2D[] explosionHits;
        private ContactFilter2D filter;
        private float explosionRadius = 5f;

        protected override void Awake()
        {
            base.Awake();

            explosionHits = new Collider2D[16];
            filter = new ContactFilter2D();
        }

        protected override void OnHitEnemy(Health health)
        {
            Despawn();
        }

        protected override void OnBeforeDespawn()
        {
            filter.SetLayerMask(hitMask);
            filter.useTriggers = true;

            int count = Physics2D.OverlapCircle(transform.position, explosionRadius,
                                                filter, explosionHits);

            for (int i = 0; i < count; i++)
            {
                if (explosionHits[i].TryGetComponent(out Health health))
                {
                    Vector2 toTarget = ((Vector2)explosionHits[i].transform.position
                                      - (Vector2)transform.position).normalized;
                    health.TakeDamage(damage, toTarget, knockbackForce);
                }
            }
        }
    }
}