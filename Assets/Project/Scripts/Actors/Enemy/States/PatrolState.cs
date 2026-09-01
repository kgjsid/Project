using UnityEngine;

using Core.System.FSM;

namespace Actors.Enemy.States
{
    public class PatrolState : IState
    {
        private EnemyContext context;

        private Vector2 patrolTarget;
        private float stuckTimer;
        private Vector2 lastPosition;
        private bool reached;
        private bool returning;

        public PatrolState(EnemyContext context)
        {
            this.context = context;
        }

        public void Enter()
        {
            patrolTarget = PickPatrolPoint();
            stuckTimer = 0f;
            lastPosition = context.transform.position;
            reached = false;
            returning = false;
        }

        public void Exit()
        {
            context.mover.Move(Vector2.zero);
        }

        public void Update()
        {
            Vector2 currentPos = context.transform.position;
            Vector2 toTarget = patrolTarget - currentPos;

            if (CheckArrivePatrolPoint(currentPos, toTarget))
            {
                reached = true;
                context.mover.Move(Vector2.zero);
                return;
            }

            CheckMovementBlocked(currentPos);

            lastPosition = currentPos;

            Vector2 dir = toTarget.normalized;
            context.mover.Move(dir);
            context.fovChecker.SetFacingDirection(dir);
        }

        public bool IsPatrolFinished()
        {
            return reached;
        }

        private bool CheckArrivePatrolPoint(Vector2 currentPos, Vector2 toTarget)
        {
            return toTarget.sqrMagnitude <= context.patrolPointArriveDistance * context.patrolPointArriveDistance;
        }

        private void CheckMovementBlocked(Vector2 currentPos)
        {
            float moved = (currentPos - lastPosition).magnitude;
            if (moved < context.searchStuckThreshold * Time.deltaTime * 60f)
            {
                stuckTimer += Time.deltaTime;
                if (stuckTimer >= context.searchStuckTime)
                {
                    HandleStuckAction();
                }
            }
            else
            {
                stuckTimer = 0f;
            }
        }

        private void HandleStuckAction()
        {
            if (!returning)
            {
                // ½ºÆù À§Ä¡·Î º¹±Í
                patrolTarget = context.spawnPosition;
                returning = true;
                stuckTimer = 0f;
            }
            else
            {
                // º¹±Í Áß¿¡µµ Á¤Áö »óÅÂ¶ó¸é ¸ØÃã(µµÂø Ã³¸®)
                reached = true;
                context.mover.Move(Vector2.zero);
            }
        }

        private Vector2 PickPatrolPoint()
        {
            Vector2 randomOffset = Random.insideUnitCircle * context.patrolRadius;
            
            return context.spawnPosition + randomOffset;
        }
    }
}