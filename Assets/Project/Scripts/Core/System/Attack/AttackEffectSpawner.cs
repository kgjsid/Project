using UnityEngine;

using Item.Data;

namespace Core.System
{
    public class AttackEffectSpawner : MonoBehaviour
    {
        [SerializeField] private Transform spawnPoint;

        private Equipper equipper;
        private IAttacker subscribedAttacker;
        private WeaponData currentWeapon;
        private Vector2 lastAimDirection = Vector2.right;

        public void Init(Equipper equipper)
        {
            this.equipper = equipper;

            equipper.OnStatsChanged += HandleWeaponChanged;
            HandleWeaponChanged();
        }

        private void OnDisable()
        {
            if (equipper != null) equipper.OnStatsChanged -= HandleWeaponChanged;
            Unsubscribe();
        }

        private void HandleWeaponChanged()
        {
            Unsubscribe();

            subscribedAttacker = equipper.GetCurrentAttacker();
            currentWeapon = equipper.GetEquippedWeapon();

            if (subscribedAttacker != null)
            {
                subscribedAttacker.OnAttackPerformed += SpawnEffect;
                subscribedAttacker.OnAimDirectionChanged += HandleAimChanged;
            }
        }

        private void Unsubscribe()
        {
            if (subscribedAttacker == null) return;

            subscribedAttacker.OnAttackPerformed -= SpawnEffect;
            subscribedAttacker.OnAimDirectionChanged -= HandleAimChanged;
            subscribedAttacker = null;
        }

        private void HandleAimChanged(Vector2 dir)
        {
            lastAimDirection = dir;
        }

        private void SpawnEffect()
        {
            if (currentWeapon == null || currentWeapon.effectPrefab == null) return;

            Vector3 pos = spawnPoint != null ? spawnPoint.position : transform.position;

            GameObject effect = Instantiate(currentWeapon.effectPrefab, pos, Quaternion.identity);
            effect.transform.right = lastAimDirection;

            if (effect.TryGetComponent(out SpriteRenderer renderer))
            {
                renderer.color = currentWeapon.effectColor;
            }
        }
    }
}