using Core.Interface;
using Core.System;
using Item.ItemObject;
using System;
using UI.Inventory;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Actors.Player
{
    public class PlayerController : ActorController
    {
        private Inventory equipInventory;

        public event Action OnRunEnded;

        private Camera mainCamera;
        private Vector3 moveDir = Vector3.zero;
        private Vector2 mouseScreenPos = Vector2.zero;

        private const int EQUIP_SLOT_COUNT = 5;

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
            Vector2 inputDir = value.Get<Vector2>();

            moveDir.x = inputDir.x;
            moveDir.z = inputDir.y;

            moveDir = moveDir.normalized;
        }

        private void OnLook(InputValue value)
        {
            mouseScreenPos = value.Get<Vector2>();

            Ray ray = mainCamera.ScreenPointToRay(mouseScreenPos);

            // 유저 위치에서 up(윗) 방향으로 plane 생성(유니티에서 계산을 위한 가상의 Plane)
            Plane groundPlane = new Plane(Vector3.up, transform.position);

            // plane.raycast -> 수학적으로 레이가 평면과 교차하는지 계산을 진행.
            // 평면의 방정식 + 벡터와의 교점 계산이라고 생각
            if (groundPlane.Raycast(ray, out float rayDistance))
            {
                Vector3 lookPoint = ray.GetPoint(rayDistance);
                Vector3 lookDir = lookPoint - transform.position;
                lookDir.y = 0;

                if (lookDir != Vector3.zero)
                {
                    mover.LookRotation(lookDir);
                    equipper.GetCurrentAttacker()?.SetAimDirection(lookDir);
                }
            }
        }

        private void OnInteract(InputValue value)
        {
            IInteractable closestInteractable = fovChecker.GetClosestTarget();

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

        private void DieRoutine()
        {
            GameObject boxObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            boxObj.name = $"{name}'s box";
            boxObj.transform.position = transform.position;
            boxObj.transform.rotation = Quaternion.identity;

            LootBox lootBox = boxObj.AddComponent<LootBox>();
            inventory.MoveItemsTo(lootBox.Inventory);

            EndRun();
        }

        public void Escape()
        {
            Debug.Log("Escape Success");
            EndRun();
        }

        private void EndRun()
        {
            enabled = false; 
            Invoke(nameof(ResetScene), 2f);
        }

        private void ResetScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public Inventory GetPlayerInventory()
        {
            return inventory;
        }
    }
}