using UnityEngine;

namespace Core.System
{
    /// <summary>
    /// Mover 컴포넌트
    /// Move를 통해 움직이는 역할 제공(CharacterController 기반)
    /// </summary>
    public class Mover : MonoBehaviour
    {
        private float baseMoveSpeed;        // 기본 이동속도
        private float bonusMoveSpeed;       // 추가된 이동속도
        private float baseRotationSpeed;

        private CharacterController controller;

        private const float ROTATE_THRESHOLD = 0.1f;

        public float BaseMoveSpeed { get { return baseMoveSpeed; } set { baseMoveSpeed = value; } }
        public float BonusMoveSpeed { get { return bonusMoveSpeed; } set { bonusMoveSpeed = value; } }
        public float BaseRotationSpeed { get { return baseRotationSpeed; } set { baseRotationSpeed = value; } }

        private void Awake()
        {
            if(!TryGetComponent<CharacterController>(out controller))
            {
                controller = gameObject.AddComponent<CharacterController>();
            }
        }

        public void Move(Vector3 movement)
        {
            controller.Move(movement * (baseMoveSpeed + BonusMoveSpeed) * Time.deltaTime);
        }

        public void LookRotation(Vector3 direction)
        {
            if (direction.sqrMagnitude < ROTATE_THRESHOLD * ROTATE_THRESHOLD) return;

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, baseRotationSpeed * Time.deltaTime);
        }
    }
}
