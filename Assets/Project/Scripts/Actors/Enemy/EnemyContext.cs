using UnityEngine;

using Core.System;

namespace Actors.Enemy
{
    /// <summary>
    /// Enemy의 State가 공유하는 데이터들.
    /// </summary>
    public class EnemyContext
    {
        public EnemyController enemyController;

        public Transform transform;
        public Mover mover;
        public Equipper equipper;

        public Transform target;
        public float attackRange;
        public float traceDist;

        public bool isTargetVisible = false;
        public Vector2 lastTargetPosition;
        public float searchArriveDistance = 0.3f;
        public float searchTimeout = 2f;
        public float searchStuckTime = 0.4f;
        public float searchStuckThreshold = 0.1f;

        public TelegraphIndicator telegraphIndicator;
        public KnockbackReceiver knockbackReceiver;
        public float telegraphDuration = 0.4f;
        public float attackRecovery = 0.3f;
        public bool isAimLocked = false;

        public float deathDuration;
    }
}