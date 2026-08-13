using System;
using UnityEngine;
using UnityEngine.InputSystem;

using Manager;
using Core.Interface;
using Core.System;
using UI.Inventory;

namespace Actors.Player
{
    public class PlayerController : ActorController
    {
        private Inventory equipInventory;

        public event Action<RunResult> OnRunEnded;

        private Camera mainCamera;
        private Vector3 moveDir = Vector3.zero;
        private Vector2 mouseScreenPos = Vector2.zero;

        private const int EQUIP_SLOT_COUNT = 7;

        public override void InitSettings()
        {
            base.InitSettings();
            mainCamera = Camera.main;
            health.OnDie += DieRoutine;
        }

        protected override void AddComponents()
        {
            base.AddComponents();
            equipInventory = gameObject.AddComponent<Inventory>();
        }

        protected override void InitStatus()
        {
            base.InitStatus();
            equipInventory.InitSlot(EQUIP_SLOT_COUNT);
            interactionDetector.SetLayerMask(LayerMask.GetMask("Interactable"));

            InventoryUI.Instance.SetTargetInventory(inventory);
            EquipUI.Instance.SetTargetInventory(equipInventory);
        }

        private void Update()
        {
            mover.Move(moveDir);
            fovChecker.FindVisibleTargets();
        }

        private void OnMove(InputValue value)
        {
            moveDir = value.Get<Vector2>().normalized;
        }

        private void OnLook(InputValue value)
        {
            mouseScreenPos = value.Get<Vector2>();

            Ray ray = mainCamera.ScreenPointToRay(mouseScreenPos);

            // 유저 위치에서 z 방향으로 plane 생성(유니티에서 계산을 위한 가상의 Plane)
            Plane groundPlane = new Plane(Vector3.forward, transform.position);

            // plane.raycast -> 수학적으로 레이가 평면과 교차하는지 계산을 진행.
            // 평면의 방정식 + 벡터와의 교점 계산이라고 생각
            if (groundPlane.Raycast(ray, out float rayDistance))
            {
                Vector3 lookPoint = ray.GetPoint(rayDistance);
                Vector2 lookDir = (Vector2)lookPoint - (Vector2)transform.position;

                if (lookDir != Vector2.zero)
                {
                    fovChecker.SetFacingDirection(lookDir);
                    equipper.GetCurrentAttacker()?.SetAimDirection(lookDir);
                }
            }
        }

        private void OnInteract(InputValue value)
        {
            IInteractable closestInteractable = interactionDetector.GetClosestTarget();

            if (closestInteractable != null)
            {
                closestInteractable.OnInteract(this);
            }
        }

        private void OnAttack(InputValue value)
        {
            equipper.GetCurrentAttacker()?.Attack();
        }

        private void OnInventory(InputValue value)
        {
            if(value.isPressed)
            {
                InventoryUI.Instance.Toggle();
            }
        }

        protected override void DieRoutine()
        {
            base.DieRoutine();

            EndRun(RunResult.Died);
        }

        public void Escape()
        {
            EndRun(RunResult.Escaped);
        }

        private void EndRun(RunResult result)
        {
            enabled = false;
            OnRunEnded?.Invoke(result);
        }

        public Inventory GetPlayerInventory()
        {
            return inventory;
        }
    }
}