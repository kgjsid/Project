using UnityEngine;

using Core.System;

namespace Actors.Enemy
{
    /// <summary>
    /// Enemy의 State가 공유하는 데이터들.
    /// </summary>
    public class EnemyContext
    {
        public Transform transform;
        public Mover mover;
        public Attacker attacker;

        public Transform target;
        public float attackRange;
        public float traceDist;
    }
}