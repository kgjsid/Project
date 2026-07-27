using UnityEngine;

namespace Actors
{
    [CreateAssetMenu(fileName = "NewActorStats", menuName = "Actor/ActorStats")]
    public class ActorStats : ScriptableObject
    {
        public float moveSpeed = 5f;
        public float rotationSpeed = 50f;
        public float maxHp = 100f;
        public float viewAngle = 45f;
        public float viewDistance = 10f;
        public int inventoryCapacity = 20;
        public float maxWeight = 20f;
    }
}