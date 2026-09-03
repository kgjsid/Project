using System;
using UnityEngine;

namespace Core.System.Attack
{
    [Serializable]
    public class AttackPattern
    {
        [SerializeField] private AttackStep[] steps;
        [SerializeField] private float totalDuration = 1f;

        public AttackStep[] Steps { get { return steps; } }
        public float TotalDuration { get { return totalDuration; } }
    }
}