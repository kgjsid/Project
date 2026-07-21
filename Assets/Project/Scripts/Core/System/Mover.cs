using UnityEngine;

namespace Core.System
{
    /// <summary>
    /// Mover 컴포넌트
    /// Move를 통해 움직이는 역할 제공
    /// </summary>
    public class Mover : MonoBehaviour
    {
        private float baseMoveSpeed;        // 기본 이동속도
        private float bonusMoveSpeed;       // 추가된 이동속도

        private Rigidbody2D rigidbody;
        private Vector2 pendingMove = Vector2.zero;

        private const float ROTATE_THRESHOLD = 0.1f;

        public float BaseMoveSpeed { get { return baseMoveSpeed; } set { baseMoveSpeed = value; } }
        public float BonusMoveSpeed { get { return bonusMoveSpeed; } set { bonusMoveSpeed = value; } }
        
        private void Awake()
        {
            if(!TryGetComponent(out rigidbody))
            {
                rigidbody = gameObject.AddComponent<Rigidbody2D>();
            }

            rigidbody.gravityScale = 0f;
            rigidbody.freezeRotation = true;
            rigidbody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }

        public void Move(Vector2 direction)
        {
            pendingMove = direction * (baseMoveSpeed + bonusMoveSpeed);
        }

        private void FixedUpdate()
        {
            rigidbody.MovePosition(rigidbody.position + pendingMove * Time.fixedDeltaTime);
        }
    }
}
