using UnityEngine;

namespace Core.System
{
    public class HomingProjectile : ProjectileBase
    {
        [SerializeField] private float turnSpeed = 180f;
        [SerializeField] private float searchRadius = 8f;
        [SerializeField] private float researchInterval = 0.2f;

        private Collider2D[] searchHits;
        private ContactFilter2D searchFilter;

        private Transform target;
        private float researchTimer;

        protected override void Awake()
        {
            base.Awake();

            searchHits = new Collider2D[16];
            searchFilter = new ContactFilter2D();
            searchFilter.useTriggers = true;
        }

        protected override void Move()
        {
            if (target == null)
            {
                researchTimer -= Time.deltaTime;
                if (researchTimer <= 0f)
                {
                    target = FindNearestTarget();
                    researchTimer = researchInterval;
                }
            }
            else
            {
                Vector2 toTarget = ((Vector2)target.position - (Vector2)transform.position).normalized;
                float maxRadians = turnSpeed * Mathf.Deg2Rad * Time.deltaTime;
                direction = Vector3.RotateTowards(direction, toTarget, maxRadians, 0f);
                transform.right = direction;
            }

            transform.position += (Vector3)(direction * speed * Time.deltaTime);
        }

        protected override void OnHitEnemy(Health health)
        {
            health.TakeDamage(damage, direction, knockbackForce);
            Despawn();
        }

        public override void OnSpawn()
        {
            base.OnSpawn();
            researchTimer = researchInterval;
        }

        public override void OnDespawn()
        {
            base.OnDespawn();
            target = null;
        }

        private Transform FindNearestTarget()
        {
            searchFilter.SetLayerMask(hitMask);

            int count = Physics2D.OverlapCircle(transform.position, searchRadius,
                                                searchFilter, searchHits);

            Transform nearest = null;
            float minSqr = float.MaxValue;

            for (int i = 0; i < count; i++)
            {
                float sqr = ((Vector2)searchHits[i].transform.position
                           - (Vector2)transform.position).sqrMagnitude;
                if (sqr < minSqr)
                {
                    minSqr = sqr;
                    nearest = searchHits[i].transform;
                }
            }

            return nearest;
        }

    }
}