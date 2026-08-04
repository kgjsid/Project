using UnityEngine;

namespace Actors
{
    [CreateAssetMenu(fileName = "NewActorStats", menuName = "Actor/ActorStats")]
    public class ActorStats : ScriptableObject
    {
        [Header("Movement")]
        public float moveSpeed = 5f;

        [Header("Health")]
        public float maxHp = 100f;
        [Range(0f, 1f)] public float knockbackResistance = 0f;

        [Header("Vision")]
        public float viewAngle = 45f;
        public float viewDistance = 10f;

        [Header("Combat")]
        public float attackRange = 2f;
        public float traceDist = 12f;
        public float telegraphDuration = 0.4f;
        public float attackRecovery = 0.3f;

        [Header("Inventory")]
        public int inventoryCapacity = 20;
        public float maxWeight = 20f;
    }
}