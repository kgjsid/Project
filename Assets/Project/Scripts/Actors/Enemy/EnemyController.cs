using UnityEngine;

using Core.System;
using Actors.Player;
using Item.ItemObject;

namespace Actors.Enemy
{
    public class EnemyController : MonoBehaviour
    {
        private Health health;
        private Mover mover;
        private FovChecker fovChecker;
        private FovRenderer fovRenderer;
        private Attacker attacker;
        private Inventory inventory;
        private Equipper equipper;

        [Header("Mover")]
        private float initialMoveSpeed = 5f;
        private float initialRotationSpeed = 50f;

        [Header("Health")]
        private float initialMaxHp = 100f;

        [Header("Fov")]
        private float initialViewAngle = 30f;
        private float initialViewDistance = 10f;
        public LayerMask targetMask;
        public LayerMask obstacleMask;

        [Header("Attack")]
        private float initialDamage = 10f;
        private float initialRange = 1f;
        private float initialTraceDist = 20f;

        [Header("Inventory")]
        private int inventoryCapacity = 5;

        private Transform target;

        private void Awake()
        {
            AddComponent();
        }

        private void Update()
        {
            if (health.IsDead()) return;

            fovChecker.FindVisibleTargets();
            CheckTarget();

            if (target != null)
            {
                float distance = Vector3.Distance(transform.position, target.position);

                if(distance <= initialRange)
                {
                    StopAndAttack();
                }
                else if(distance <= initialTraceDist)
                {
                    TraceTarget();
                }
                else
                {
                    target = null;
                }
            }
        }

        private void AddComponent()
        {
            health = gameObject.AddComponent<Health>();
            mover = gameObject.AddComponent<Mover>();
            fovChecker = gameObject.AddComponent<FovChecker>();
            attacker = gameObject.AddComponent<Attacker>();
            inventory = gameObject.AddComponent<Inventory>(); 
            equipper = gameObject.AddComponent<Equipper>();

            GameObject fovObject = new GameObject("FovMesh");
            fovObject.transform.parent = transform;
            fovObject.transform.localPosition = Vector3.zero;
            fovObject.transform.rotation = Quaternion.identity;
            fovRenderer = fovObject.AddComponent<FovRenderer>();

            InitStatus();

            fovRenderer.Chekcer = fovChecker;
            fovRenderer.SetColor(new Color(1f, 0.2f, 0f, 0.3f));

            health.OnDie += DieRoutine;
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
            equipper.Init(health, mover, attacker);
        }

        private void CheckTarget()
        {
            foreach(var visible in fovChecker.VisibleTargets)
            {
                if(visible.TryGetComponent(out PlayerController player))
                {
                    target = player.transform;
                    return;
                }
            }
        }

        private void TraceTarget()
        {
            Vector3 direction = (target.position - transform.position).normalized;
            mover.Move(direction);
            mover.LookRotation(direction);
        }

        private void StopAndAttack()
        {
            mover.Move(Vector3.zero);
            attacker.Attack();
        }

        private void DieRoutine()
        {
            GameObject boxObj = new GameObject($"{name}'s box");
            boxObj.transform.position = transform.position;
            boxObj.transform.rotation = Quaternion.identity;

            LootBox lootBox = boxObj.AddComponent<LootBox>();
            inventory.MoveItemsTo(lootBox.Inventory);
        }
    }
}