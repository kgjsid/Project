using UnityEngine;

using Core.System.Pooling;

namespace Core.System
{
    [RequireComponent(typeof(Collider2D))]
    public abstract class ProjectileBase : MonoBehaviour, IPoolable
    {
        [SerializeField] private float lifeTime = 3f;
        [SerializeField] private SpriteRenderer spriteRenderer;

        protected Vector2 direction;
        protected float speed;
        protected float damage;
        protected float knockbackForce;

        private LayerMask hitMask;
        private LayerMask obstacleMask;

        private float lifeTimer;
        private bool isActive;

        protected virtual void Awake()
        {
            if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void Launch(Vector2 direction, float speed, float damage,
                           float knockbackForce, LayerMask hitMask, LayerMask obstacleMask,
                           Sprite sprite, Color color)
        {
            this.direction = direction;
            this.speed = speed;
            this.damage = damage;
            this.knockbackForce = knockbackForce;
            this.hitMask = hitMask;
            this.obstacleMask = obstacleMask;

            transform.right = this.direction;

            if (spriteRenderer != null)
            {
                if (sprite != null) spriteRenderer.sprite = sprite;
                spriteRenderer.color = color;
            }

            lifeTimer = lifeTime;
            isActive = true;
        }

        private void Update()
        {
            if (!isActive) return;

            Move();

            lifeTimer -= Time.deltaTime;
            if (lifeTimer <= 0f) Despawn();
        }

        protected virtual void Move()
        {
            transform.position += (Vector3)(direction * speed * Time.deltaTime);
        }

        public void OnDespawn()
        {
            isActive = false;
            direction = Vector2.zero;
        }

        public void OnSpawn()
        {

        }

        protected void Despawn()
        {
            isActive = false;
            PoolManager.Instance.Return((IPoolable)this);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!isActive) return;

            int otherLayer = 1 << other.gameObject.layer;

            if ((otherLayer & obstacleMask) != 0)
            {
                Despawn();
                return;
            }

            if ((otherLayer & hitMask) != 0)
            {
                if (other.TryGetComponent(out Health health))
                {
                    OnHitEnemy(health);
                }
            }
        }

        protected abstract void OnHitEnemy(Health health);
    }
}