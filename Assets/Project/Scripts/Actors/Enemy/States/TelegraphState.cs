using UnityEngine;

using Core.System.FSM;
using Item.Data;

namespace Actors.Enemy.States
{
    public class TelegraphState : IState
    {
        private EnemyContext context;

        private float timer;

        public TelegraphState(EnemyContext context)
        {
            this.context = context;
        }

        public void Enter()
        {
            timer = context.telegraphDuration;
            context.mover.Move(Vector2.zero);

            if (context.knockbackReceiver != null)
            {   
                // 공격 대기 중에는 넉백 면역
                context.knockbackReceiver.IsImmune = true;
            }

            if (context.target != null)
            {
                Vector2 dir = ((Vector2)context.target.position - (Vector2)context.transform.position).normalized;
                context.telegraphIndicator?.SetDirection(dir);
                context.equipper.GetCurrentAttacker()?.SetAimDirection(dir);
                context.isAimLocked = true;
            }

            WeaponData weapon = context.equipper.GetEquippedWeapon();
            context.telegraphIndicator?.SetRange(weapon != null ? weapon.range : context.attackRange);
            context.telegraphIndicator?.ShowIndicator();
        }

        public void Exit()
        {
            if (context.knockbackReceiver != null)
            {
                context.knockbackReceiver.IsImmune = false;
            }

            context.telegraphIndicator?.HideIndicator();
            context.isAimLocked = false;
        }

        public void Update()
        {
            timer -= Time.deltaTime;
            context.mover.Move(Vector2.zero);

            float t = 1f - Mathf.Clamp01(timer / context.telegraphDuration);
            context.telegraphIndicator?.SetProgress(t);
        }

        public bool IsFinished()
        {
            return timer <= 0f;
        }
    }
}