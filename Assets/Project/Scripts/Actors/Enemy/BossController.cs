using UnityEngine;

using Item.Data;

namespace Actors.Enemy
{
    public class BossController : EnemyController
    {
        [System.Serializable]
        public class BossPhase
        {
            [Range(0f, 1f)] public float enterHpRatio;
            public WeaponData weapon;
            public float attackRange;
        }

        [SerializeField] private BossPhase[] phases;

        private int currentPhaseIndex = -1;

        public override void InitSettings()
        {
            base.InitSettings();

            health.OnHpChanged += CheckPhase;

            if (phases != null && phases.Length > 0)
            {
                currentPhaseIndex++;
                ApplyPhase(currentPhaseIndex);
            }
        }

        private void CheckPhase(float hpRatio)
        {
            while (currentPhaseIndex + 1 < phases.Length && hpRatio <= phases[currentPhaseIndex + 1].enterHpRatio)
            {
                currentPhaseIndex++;
                ApplyPhase(currentPhaseIndex);
            }
        }

        private void ApplyPhase(int index)
        {
            BossPhase phase = phases[index];

            if (phase.weapon != null)
                equipper.Equip(phase.weapon);

            if (context != null && phase.attackRange > 0f)
                context.attackRange = phase.attackRange;
        }

        private void OnDisable()
        {
            if (health != null)
                health.OnHpChanged -= CheckPhase;
        }
    }
}