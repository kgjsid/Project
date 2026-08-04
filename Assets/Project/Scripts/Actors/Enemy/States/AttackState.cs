using UnityEngine;

using Core.System.FSM;

namespace Actors.Enemy.States
{
    public class AttackState : IState
    {
        private EnemyContext context;
        private float timer;

        public AttackState(EnemyContext context)
        {
            this.context = context;
        }

        public void Enter()
        {
            context.mover.Move(Vector2.zero);
            context.equipper.GetCurrentAttacker()?.ForceAttack();
            timer = context.attackRecovery;
        }

        public void Exit()
        {

        }

        public void Update()
        {
            context.mover.Move(Vector3.zero);
            timer -= Time.deltaTime;
        }

        public bool IsFinished()
        {
            return timer <= 0f;
        }
    }
}