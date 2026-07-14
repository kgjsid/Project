using UnityEngine;
using UnityEngine.InputSystem;

using Core.System;
using Core.Interface;
using UI.Inventory;

namespace Actors.Player
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Comp")]
        private Mover mover;
        private Health health;
        private FovChecker fovChecker;
        private FovRenderer fovRenderer;
        private Attacker attacker;
        private Inventory inventory;
        private Inventory equipInventory;
        private Equipper equipper;

        [Header("Mover")]
        private float initialMoveSpeed = 10f;
        private float initialRotationSpeed = 50f;

        [Header("Health")]
        private float initialMaxHp = 100f;

        [Header("Fov")]
        private float initialViewAngle = 45f;
        private float initialViewDistance = 10f;
        public LayerMask targetMask;
        public LayerMask obstacleMask;

        [Header("Attack")]
        private float initialDamage = 10f;
        private float initialRange = 5f;

        [Header("Inventory")]
        private int inventoryCapacity = 20;

        private Camera mainCamera;
        private Vector3 moveDir = Vector3.zero;
        private Vector2 mouseScreenPos = Vector2.zero;

        private const int EQUIP_SLOT_COUNT = 5;

        private void Start()
        {
            AddComponent();

            mainCamera = Camera.main;

            health.OnDie += DieRoutine;
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
            attacker.Attack();
        }

        private void OnInventory(InputValue value)
        {
            if(value.isPressed)
            {
                InventoryUI.Instance.Toggle();
            }
        }

        private void AddComponent()
        {
            mover = gameObject.AddComponent<Mover>();
            health = gameObject.AddComponent<Health>();
            fovChecker = gameObject.AddComponent<FovChecker>();
            attacker = gameObject.AddComponent<Attacker>();
            inventory = gameObject.AddComponent<Inventory>();
            equipInventory = gameObject.AddComponent<Inventory>();
            equipper = gameObject.AddComponent<Equipper>();

            GameObject fovObject = new GameObject("FovMesh");
            fovObject.transform.parent = transform;
            fovObject.transform.localPosition = Vector3.zero;
            fovObject.transform.rotation = Quaternion.identity;
            fovRenderer = fovObject.AddComponent<FovRenderer>();

            InitStatus();

            InventoryUI.Instance.SetTargetInventory(inventory);
            EquipUI.Instance.SetTargetInventory(equipInventory);

            fovRenderer.Chekcer = fovChecker;
        }

        private void InitStatus()
        {
            mover.BaseMoveSpeed = initialMoveSpeed;
            mover.BaseRotationSpeed = initialRotationSpeed;

            health.BaseMaxHp = initialMaxHp;
            health.CurrentHp = initialMaxHp;

            fovChecker.ViewAngle = initialViewAngle;
            fovChecker.ViewDistance = initialViewDistance;
            fovChecker.TargetMask = targetMask;
            fovChecker.ObstacleMask = obstacleMask;

            inventory.InitSlot(inventoryCapacity);
            equipInventory.InitSlot(EQUIP_SLOT_COUNT);
            equipper.Init(health, mover, attacker);
        }

        private void DieRoutine()
        {

        }

        public Inventory GetPlayerInventory()
        {
            return inventory;
        }
    }
}