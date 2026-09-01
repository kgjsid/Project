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
        private Vector2 targetMovement = Vector2.zero;
        private Vector2 knockbackVelocity;
        private float knockbackTimer;
        private float currentMoveSpeed;
        private bool isFrozen;

        private Vector2 dashVelocity;
        private float dashTimer;

        private const float KNOCKBACK_THRESHOLD = 0.0001f;

        public float BaseMoveSpeed { get { return baseMoveSpeed; } set { baseMoveSpeed = value; } }
        public float BonusMoveSpeed { get { return bonusMoveSpeed; } set { bonusMoveSpeed = value; } }
        public float CurrentMoveSpeed { get { return currentMoveSpeed; } }
        public bool IsFrozen { get { return isFrozen; } set { isFrozen = value; moverRigidbody.bodyType = value ? RigidbodyType2D.Kinematic : RigidbodyType2D.Dynamic; } }
        public bool IsDashing { get { return dashTimer > 0f; } }

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
            targetMovement = direction * (baseMoveSpeed + bonusMoveSpeed);

            currentMoveSpeed = targetMovement.magnitude;
        }

        public void ApplyKnockback(Vector2 direction, float force, float duration = 0.15f)
        {
            if (direction.sqrMagnitude < KNOCKBACK_THRESHOLD) return;

            knockbackVelocity = direction.normalized * force;
            knockbackTimer = duration;
        }

        public void ApplyDash(Vector2 direction, float speed, float duration)
        {
            if (direction.sqrMagnitude < KNOCKBACK_THRESHOLD) return;
            dashVelocity = direction.normalized * speed;
            dashTimer = duration;
        }

        private void FixedUpdate()
        {
            if (IsFrozen) return;

            Vector2 velocity = targetMovement;

            if (dashTimer > 0f)
            {
                velocity = dashVelocity;
                dashTimer -= Time.fixedDeltaTime;
            }
            else
            {
                if (knockbackTimer > 0f)
                {
                    velocity += knockbackVelocity;
                    knockbackTimer -= Time.fixedDeltaTime;

                    knockbackVelocity = Vector2.Lerp(knockbackVelocity, Vector2.zero, Time.fixedDeltaTime * 8f);

                    if (knockbackTimer <= 0f) knockbackVelocity = Vector2.zero;
                }
            }
            moverRigidbody.MovePosition(moverRigidbody.position + velocity * Time.fixedDeltaTime);
        }
    }
}
