using UnityEngine;

using Core.System.FSM;

namespace Actors.Enemy.States
{
    public class DieState : IState
    {
        private EnemyContext context;
        public DieState(EnemyContext context)
        {
            this.context = context;
        }

        public void Enter()
        {
            context.mover.Move(Vector3.zero);
        }

        public void Exit()
        {
        }

        public void Update()
        {
        }
    }
}