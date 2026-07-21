using UnityEngine;

namespace Core.System
{
    public class Projectile : MonoBehaviour
    {
        private Vector2 direction;
        private float speed;
        private float damage;
        private LayerMask hitMask;

        public void Launch(Vector2 direction, float speed, float damage, LayerMask hitMask)
        {
            this.direction = direction;
            this.speed = speed;
            this.damage = damage;
            this.hitMask = hitMask;
            transform.right = direction;
        }

        private void Update()
        {
            transform.position += (Vector3)(direction * speed * Time.deltaTime);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (((1 << other.gameObject.layer) & hitMask) == 0) return;

            if (other.TryGetComponent(out Health health))
            {
                health.TakeDamage(damage);
            }
            
            Destroy(gameObject);
        }
    }
}