using UnityEngine;

using Core.System.FSM;

namespace Actors.Enemy.States
{
    public class IdleState : IState
    {
        private EnemyContext context;

        private float lookTimer;
        private float changeTimer;
        private float rotateDir;
        private float currentAngle;

        public IdleState(EnemyContext context)
        {
            this.context = context;
        }

        public void Enter()
        {
            lookTimer = 0f;
            changeTimer = 0f;
            rotateDir = Random.value < 0.5f ? 1f : -1f;

            Vector2 facing = context.fovChecker.FacingDirection;
            currentAngle = Mathf.Atan2(facing.y, facing.x) * Mathf.Rad2Deg;
        }

        public void Exit()
        {

        }

        public void Update()
        {
            context.mover.Move(Vector3.zero);

            lookTimer += Time.deltaTime;
            changeTimer += Time.deltaTime;

            if(changeTimer >= context.scanChangeInterval)
            {
                changeTimer = 0f;
                rotateDir = Random.value < 0.5f ? 1f : -1f;

                if(Random.value < 0.3f)
                {
                    currentAngle += Random.Range(-90f, 90f);
                }
            }

            currentAngle += context.scanRotateSpeed * rotateDir * Time.deltaTime;

            float rad = currentAngle * Mathf.Deg2Rad;
            Vector2 lookDir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
            context.fovChecker.SetFacingDirection(lookDir);
        }

        public bool CheckStartPatrol()
        {
            return lookTimer >= context.idleDuration;
        }
    }
}