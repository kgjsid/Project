using UnityEngine;

namespace Core.System
{
    public class ActorAnimator : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private SpriteRenderer spriteRenderer;

        private Mover mover;
        private Health health;
        private Equipper equipper;
        private FovChecker fovChecker;
        private IAttacker subscribedAttacker;
        private bool isDead;

        private const string SPEED_PARAMETER = "Speed";
        private const string ATTACK_TRIGGER = "Attack";
        private const string HURT_TRIGGER = "Hurt";
        private const string DIE_TRIGGHER = "Die";

        public void Init(Mover mover, Health health, Equipper equipper, FovChecker fovChecker)
        {
            this.mover = mover;
            this.health = health;
            this.equipper = equipper;
            this.fovChecker = fovChecker;

            if (health != null)
            {
                health.OnDie += HandleDieAnimation;
                health.OnDamaged += HandleHpChanged;
            }
            if (equipper != null)
            {
                equipper.OnStatsChanged += HandleWeaponChanged;
                HandleWeaponChanged();
            }
        }

        private void OnDisable()
        {
            if (health != null)
            {
                health.OnDie -= HandleDieAnimation;
            }
            if (equipper != null)
            {
                equipper.OnStatsChanged -= HandleWeaponChanged;
            }
            UnsubscribeAttacker();
        }

        private void Update()
        {
            if (mover == null) return;
            if (isDead) return;

            animator.SetFloat(SPEED_PARAMETER, mover.CurrentMoveSpeed);

            if (Mathf.Abs(fovChecker.FacingDirection.x) > 0.01f)
            {
                spriteRenderer.flipX = fovChecker.FacingDirection.x < 0;
            }
        }

        private void HandleWeaponChanged()
        {
            UnsubscribeAttacker();
            subscribedAttacker = equipper.GetCurrentAttacker();
            if (subscribedAttacker != null)
            {
                subscribedAttacker.OnAttackPerformed += HandleAttackAnimation;
            }
        }

        private void UnsubscribeAttacker()
        {
            if (subscribedAttacker != null)
            {
                subscribedAttacker.OnAttackPerformed -= HandleAttackAnimation;
            }
        }

        private void HandleHpChanged(float damage, Vector2 hitDirection, float knockbackForce)
        {
            if (!health.IsDead()) animator.SetTrigger(HURT_TRIGGER);
        }

        private void HandleAttackAnimation()
        {
            animator.SetTrigger(ATTACK_TRIGGER);
        }

        private void HandleDieAnimation()
        {
            isDead = true;
            animator.SetTrigger(DIE_TRIGGHER);
        }
    }
}