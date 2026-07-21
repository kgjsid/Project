using UnityEngine;

using Core.System.FSM;

namespace Actors.Enemy.States
{
    public class AttackState : IState
    {
        private EnemyContext context;
        public AttackState(EnemyContext context)
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
            context.equipper.GetCurrentAttacker()?.Attack();
        }
    }
}