using UnityEngine;

using Core.System;
using Core.System.Pooling;

namespace UI.HUD
{
    public class DamageNumberSpawner : MonoBehaviour
    {
        [SerializeField] private Vector3 spawnOffset = new Vector3(0f, 0.5f, 0f);

        private Health health;

        public void Init(Health health)
        {
            this.health = health;
            health.OnDamaged += HandleDamaged;
        }

        private void OnDisable()
        {
            if (health != null) health.OnDamaged -= HandleDamaged;
        }

        private void HandleDamaged(float damage, Vector2 hitDirection, float knockbackForce)
        {
            if (damage <= 0f) return;

            if (PoolManager.Instance == null) return;

            Debug.Log("데미지 생성");

            DamageNumber number = PoolManager.Instance.Get<DamageNumber>();
            if(number != null)
            {
                number.Show(damage, transform.position + spawnOffset);
            }
        }
    }
}