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
        public FovChecker fovChecker;

        // AttackState
        public Transform target;
        public float attackRange;
        public float traceDist;

        // IdleState
        public float idleDuration = 2f;
        public float scanRotateSpeed = 60f;
        public float scanChangeInterval = 1.2f;

        // SearchState
        public bool isTargetVisible = false;
        public Vector2 lastTargetPosition;
        public float searchArriveDistance = 0.3f;
        public float searchTimeout = 2f;
        public float searchStuckTime = 0.4f;
        public float searchStuckThreshold = 0.1f;

        // PatrolState
        public Vector2 spawnPosition;
        public float patrolRadius = 4f;
        public float patrolPointArriveDistance = 0.3f;

        // TelegraphState
        public TelegraphIndicator telegraphIndicator;
        public KnockbackReceiver knockbackReceiver;
        public float telegraphDuration = 0.4f;
        public float attackRecovery = 0.3f;
        public bool isAimLocked = false;

        public float deathDuration;
    }
}