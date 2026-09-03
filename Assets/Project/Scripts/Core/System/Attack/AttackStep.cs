using System;

namespace Core.System.Attack
{
    public enum AttackStepType { Melee, Projectile, Charge, Shockwave, Wait}

    [Serializable]
    public struct AttackStep
    {
        public AttackStepType type;
        public float startTime;
    }
}