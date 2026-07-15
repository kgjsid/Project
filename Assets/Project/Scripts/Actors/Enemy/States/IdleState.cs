using UnityEngine;

using Core.System.FSM;

namespace Actors.Enemy.States
{
    public class IdleState : IState
    {
        private EnemyContext context;

        public IdleState(EnemyContext context)
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
            context.mover.Move(Vector3.zero);
        }
    }
}