using UnityEngine;

using Core.System.Pooling;

namespace Core.System
{
    public class Projectile : MonoBehaviour, IPoolable
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private float lifeTime = 3f;

        private Vector2 direction;
        private float speed;
        private float damage;
        private float knockbackForce;
        private LayerMask hitMask;
        private LayerMask obstacleMask;

        private float lifeTimer;
        private bool isActive;

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

        public void OnSpawn()
        {

        }

        public void OnDespawn()
        {
            isActive = false;
            direction = Vector2.zero;
        }

        private void Update()
        {
            if (!isActive) return;

            transform.position += (Vector3)(direction * speed * Time.deltaTime);

            lifeTimer -= Time.deltaTime;
            if (lifeTimer <= 0f) Despawn();
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
                    health.TakeDamage(damage, direction, knockbackForce);
                }
                Despawn();
            }
        }

        private void Despawn()
        {
            isActive = false;
            PoolManager.Instance.Return(this);
        }
    }
}