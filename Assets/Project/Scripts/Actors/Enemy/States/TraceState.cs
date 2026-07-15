using UnityEngine;

using Core.System.FSM;

namespace Actors.Enemy.States
{
    public class TraceState : IState
    {
        private EnemyContext context;
        public TraceState(EnemyContext context)
        {
            this.context = context;
        }
        public void Enter()
        {
        }

        public void Exit()
        {
        }

        public void Update()
        {
            if (context.target == null) return;

            Vector3 dir = (context.target.position - context.transform.position).normalized;
            context.mover.Move(dir);
            context.mover.LookRotation(dir);
        }
    }
}