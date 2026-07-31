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

        private Rigidbody2D moverRigidbody;
        private Vector2 pendingMove = Vector2.zero;
        private Vector2 knockbackVelocity;
        private float knockbackTimer;
        private float currentMoveSpeed;

        private const float KNOCKBACK_THRESHOLD = 0.0001f;

        public float BaseMoveSpeed { get { return baseMoveSpeed; } set { baseMoveSpeed = value; } }
        public float BonusMoveSpeed { get { return bonusMoveSpeed; } set { bonusMoveSpeed = value; } }
        public float CurrentMoveSpeed { get { return currentMoveSpeed; } }

        private void Awake()
        {
            if(!TryGetComponent(out moverRigidbody))
            {
                moverRigidbody = gameObject.AddComponent<Rigidbody2D>();
            }

            moverRigidbody.gravityScale = 0f;
            moverRigidbody.freezeRotation = true;
            moverRigidbody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }

        public void Move(Vector2 direction)
        {
            pendingMove = direction * (baseMoveSpeed + bonusMoveSpeed);

            currentMoveSpeed = pendingMove.magnitude;
        }

        public void ApplyKnockback(Vector2 direction, float force, float duration = 0.15f)
        {
            if (direction.sqrMagnitude < KNOCKBACK_THRESHOLD) return;

            knockbackVelocity = direction.normalized * force;
            knockbackTimer = duration;
        }

        private void FixedUpdate()
        {
            Vector2 velocity = pendingMove;

            if (knockbackTimer > 0f)
            {
                velocity += knockbackVelocity;
                knockbackTimer -= Time.fixedDeltaTime;

                knockbackVelocity = Vector2.Lerp(knockbackVelocity, Vector2.zero, Time.fixedDeltaTime * 8f);

                if (knockbackTimer <= 0f) knockbackVelocity = Vector2.zero;
            }

            moverRigidbody.MovePosition(moverRigidbody.position + velocity * Time.fixedDeltaTime);
        }
    }
}
