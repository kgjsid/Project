using UnityEngine;

using Core.System.FSM;
using Core.System.Attack;

namespace Actors.Enemy.States
{
    public class AttackState : IState
    {
        private EnemyContext context;
        private float attackTimer;
        private bool[] executed;
        private AttackPattern pattern;
        private Vector2 lockedAim;

        public AttackState(EnemyContext context)
        {
            this.context = context;
        }

        public void Enter()
        {
            context.mover.Move(Vector2.zero);
            attackTimer = 0f;

            var weapon = context.equipper.GetEquippedWeapon();
            pattern = weapon?.attackPattern;

            if(pattern == null || pattern.Steps == null || pattern.Steps.Length == 0)
            {
                context.equipper.GetCurrentAttacker()?.ForceAttack();
                executed = null;
                return;
            }

            executed = new bool[pattern.Steps.Length];
            AttackStepExecutor.SetupPattern(context, weapon, pattern.Steps);

            lockedAim = context.target != null
                ? ((Vector2)context.target.position - (Vector2)context.transform.position).normalized
                : context.fovChecker.FacingDirection;
        }

        public void Exit()
        {

        }

        public void Update()
        {
            attackTimer += Time.deltaTime;
            if (executed == null) return;

            for(int i = 0; i < pattern.Steps.Length; i++)
            {
                if (executed[i]) continue;
                if (attackTimer >= pattern.Steps[i].startTime)
                {
                    lockedAim = context.target != null
                        ? ((Vector2)context.target.position - (Vector2)context.transform.position).normalized
                        : context.fovChecker.FacingDirection;

                    AttackStepExecutor.Execute(context, pattern.Steps[i], lockedAim);
                    executed[i] = true;
                }
            }
        }

        public bool IsFinished()
        {
            return executed == null
                ? attackTimer >= context.attackRecovery
                : attackTimer >= pattern.TotalDuration;
        }
    }
}