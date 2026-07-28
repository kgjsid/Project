using Core.Interface;
using Core.System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Actors.Player
{
    public class HubPlayerController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 6f;

        [Header("Animator")]
        [SerializeField] protected ActorAnimator actorAnimator;

        private Mover mover;
        private InteractionDetector detector;
        private FovChecker fovChecker;
        private Vector2 moveDir;

        private void Awake()
        {
            mover = gameObject.AddComponent<Mover>();
            detector = gameObject.AddComponent<InteractionDetector>();
            fovChecker = gameObject.AddComponent<FovChecker>();

            actorAnimator.Init(mover, null, null, fovChecker);

            mover.BaseMoveSpeed = moveSpeed;
        }

        private void Update()
        {
            mover.Move(moveDir);
        }

        private void OnMove(InputValue value)
        {
            moveDir = value.Get<Vector2>().normalized;
        }

        private void OnInteract(InputValue value)
        {
            if (!value.isPressed) return;

            IInteractable target = detector.GetClosestTarget();
            target?.OnInteract(null);
        }
    }
}