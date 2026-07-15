using UnityEngine;

using Core.System;

namespace Actors
{
    /// <summary>
    /// Player, Enemy, NPC 등 공통으로 가질 컴포넌트 담당
    /// </summary>
    public abstract class ActorController : MonoBehaviour
    {
        [Header("Stats")]
        [SerializeField] protected ActorStats stats;

        [Header("LayerMask")]
        public LayerMask targetMask;
        public LayerMask obstacleMask;

        protected Health health;
        protected Mover mover;
        protected FovChecker fovChecker;
        protected FovRenderer fovRenderer;
        protected Attacker attacker;
        protected Inventory inventory;
        protected Equipper equipper;

        protected void Awake()
        {
            InitSettings();
        }

        public virtual void InitSettings()
        {
            AddComponents();
            InitStatus();
        }

        /// <summary>
        /// 컴포넌트 추가
        /// </summary>
        protected virtual void AddComponents()
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
            fovRenderer.Chekcer = fovChecker;
        }

        /// <summary>
        /// 실제 스탯 적용
        /// </summary>
        protected virtual void InitStatus()
        {
            mover.BaseMoveSpeed = stats.moveSpeed;
            mover.BaseRotationSpeed = stats.rotationSpeed;

            health.BaseMaxHp = stats.maxHp;
            health.CurrentHp = stats.maxHp;

            fovChecker.ViewAngle = stats.viewAngle;
            fovChecker.ViewDistance = stats.viewDistance;
            fovChecker.TargetMask = targetMask;
            fovChecker.ObstacleMask = obstacleMask;

            attacker.SetLayerMask(targetMask);

            inventory.InitSlot(stats.inventoryCapacity);
            equipper.Init(health, mover, attacker);
        }
    }
}
