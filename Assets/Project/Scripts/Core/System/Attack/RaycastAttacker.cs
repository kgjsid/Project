using System;
using UnityEngine;

using Item.Data;

namespace Core.System
{
    public class RaycastAttacker : MonoBehaviour, IAttacker
    {
        private float damage;
        private float range;
        private float attackSpeed = 1f;
        private float knockbackForce;
        private float boxWidth = 0.3f;
        private int beamCount = 1;
        private float spreadAngle = 0f;
        private bool hasWeapon;
        private float lastAttackTime = 1f;
        private Vector2 aimDirection = Vector2.right;

        private ContactFilter2D hitFilter;
        private ContactFilter2D obstacleFilter;
        private RaycastHit2D[] hitResults;
        private RaycastHit2D[] obstacleResult;

        private BeamRenderer beamRenderer;
        private Vector3[] beamEnds;

        private const int HIT_RESULT_COUNT = 16;
        private const float ROTATION_THRESHOLD = 0.0001f;
        private const int MAX_BEAM_COUNT = 8;

        public event Action OnAttackPerformed;
        public event Action<Vector2> OnAimDirectionChanged;

        private void Awake()
        {
            hitResults = new RaycastHit2D[HIT_RESULT_COUNT];
            obstacleResult = new RaycastHit2D[1];

            beamEnds = new Vector3[MAX_BEAM_COUNT];
        }

        public void Init(BeamRenderer beamRenderer)
        {
            this.beamRenderer = beamRenderer;
        }

        public void SetWeapon(WeaponData weapon)
        {
            damage = weapon.raycast.damage;
            range = weapon.raycast.range;
            attackSpeed = weapon.raycast.attackSpeed;
            knockbackForce = weapon.raycast.knockbackForce;
            boxWidth = weapon.raycast.boxWidth;
            beamCount = Mathf.Max(1, weapon.raycast.beamCount);
            spreadAngle = weapon.raycast.spreadAngle;
            hasWeapon = true;
        }

        public void ClearWeapon()
        {
            hasWeapon = false;
        }

        public void SetLayerMask(LayerMask hitMask)
        {
            hitFilter = new ContactFilter2D();
            hitFilter.SetLayerMask(hitMask);
            hitFilter.useTriggers = true;
        }

        public void SetObstacleMask(LayerMask obstacleMask)
        {
            obstacleFilter = new ContactFilter2D();
            obstacleFilter.SetLayerMask(obstacleMask);
            obstacleFilter.useTriggers = true;
        }

        public void SetAimDirection(Vector2 direction)
        {
            if (direction.sqrMagnitude < ROTATION_THRESHOLD) return;
            aimDirection = direction.normalized;
            OnAimDirectionChanged?.Invoke(aimDirection);
        }

        public void Attack()
        {
            if (!hasWeapon) return;
            if (Time.time - lastAttackTime < 1f / attackSpeed) return;
            ExecuteAttack();
        }

        public void ForceAttack()
        {
            if (!hasWeapon) return;
            ExecuteAttack();
        }

        private void ExecuteAttack()
        {
            lastAttackTime = Time.time;

            float startAngle = -spreadAngle * 0.5f;
            float step = beamCount > 1 ? spreadAngle / (beamCount - 1) : 0f;

            for(int i = 0; i < beamCount; i++)
            {
                float angle = startAngle + step * i;
                Vector2 dir = Rotate(aimDirection, angle);
                FireBeam(dir, i);
            }

            if (beamRenderer != null)
            {
                beamRenderer.ShowBeams(transform.position, beamEnds, beamCount, boxWidth);
            }

            OnAttackPerformed?.Invoke();
        }

        private void FireBeam(Vector2 dir, int beamIndex)
        {
            float boxAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            Vector2 boxSize = new Vector2(boxWidth, boxWidth);

            float effectiveRange = range;
            int obstacleCount = Physics2D.BoxCast(transform.position, boxSize, boxAngle,
                    dir, obstacleFilter, obstacleResult, range);

            if (obstacleCount > 0)
            {
                effectiveRange = obstacleResult[0].distance;
            }

            int count = Physics2D.BoxCast(transform.position, boxSize, boxAngle,
                              dir, hitFilter, hitResults, effectiveRange);

            beamEnds[beamIndex] = transform.position + (Vector3)(dir * effectiveRange);

            for (int i = 0; i < count; i++)
            {
                if (hitResults[i].collider.TryGetComponent(out Health health))
                {
                    health.TakeDamage(damage, dir, knockbackForce);
                }
            }
        }

        private Vector2 Rotate(Vector2 v, float degrees)
        {
            float rad = degrees * Mathf.Deg2Rad;
            float cos = Mathf.Cos(rad);
            float sin = Mathf.Sin(rad);
            return new Vector2(v.x * cos - v.y * sin, v.x * sin + v.y * cos);
        }
    }
}