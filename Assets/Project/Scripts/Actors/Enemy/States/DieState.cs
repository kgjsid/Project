using UnityEngine;

using Core.System.FSM;

namespace Actors.Enemy.States
{
    public class DieState : IState
    {
        private EnemyContext context;
        private float timer;

        public DieState(EnemyContext context)
        {
            this.context = context;
        }

        public void Enter()
        {
            context.mover.Move(Vector3.zero);

            timer = context.deathDuration;
        }

        public void Exit()
        {
        }

        public void Update()
        {
            timer -= Time.deltaTime;

            if (timer <= 0f)
            {
                context.enemyController.SpawnLootBox();
            }
        }
    }
}