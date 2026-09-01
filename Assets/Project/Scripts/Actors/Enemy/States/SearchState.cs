using UnityEngine;

using Core.System.FSM;

namespace Actors.Enemy.States
{
    public class SearchState : IState
    {
        private EnemyContext context;

        private float searchTimer;
        private float stuckTimer;
        private Vector2 lastPosition;

        public SearchState(EnemyContext context)
        {
            this.context = context;
        }

        public void Enter()
        {
            searchTimer = 0f;
            stuckTimer = 0f;
            lastPosition = context.transform.position;
        }

        public void Exit()
        {
            context.mover.Move(Vector2.zero);
        }

        public void Update()
        {
            searchTimer += Time.deltaTime;

            if(searchTimer >= context.searchTimeout)
            {
                SearchFailed();
                return;
            }

            Vector2 currentPos = context.transform.position;

            if (CheckArriveDistance(currentPos))
            {
                SearchFailed();
                return;
            }

            if(IsMovementBlocked(currentPos))
            {
                SearchFailed();
                return;
            }

            lastPosition = currentPos;
            context.mover.Move((context.lastTargetPosition - currentPos).normalized);
        }

        private bool CheckArriveDistance(Vector2 currentPos)
        {
            Vector2 lastPos = context.lastTargetPosition - currentPos;

            return lastPos.SqrMagnitude() <= context.searchArriveDistance * context.searchArriveDistance;
        }

        private bool IsMovementBlocked(Vector2 currentPos)
        {
            float movedDistance = ((Vector2)currentPos - lastPosition).magnitude;
            if(movedDistance < context.searchStuckThreshold * Time.deltaTime * 60f)
            {
                stuckTimer += Time.deltaTime;
                return stuckTimer >= context.searchStuckTime;
            }
            else
            {
                stuckTimer = 0f;
                return false;
            }
        }

        private void SearchFailed()
        {
            context.mover.Move(Vector2.zero);
            context.target = null;
        }
    }
}