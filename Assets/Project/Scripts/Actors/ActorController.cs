using Core.System;
using Item.ItemObject;
using UI.HUD;
using UnityEngine;

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

        [Header("LootBox Prefab")]
        [SerializeField] protected LootBox lootBoxPrefab;

        [Header("Animator")]
        [SerializeField] protected ActorAnimator actorAnimator;

        [Header("Effects")]
        [SerializeField] protected HitFlash hitFlash;
        [SerializeField] protected WeaponAimer weaponAimer;
        [SerializeField] protected AttackEffectSpawner attackEffectSpawner;
        [SerializeField] protected BeamRenderer beamRenderer;
        [SerializeField] protected HitStop hitStop;

        [Header("HUD")]
        [SerializeField] protected HealthBarUI healthBar;
        [SerializeField] protected DamageNumberSpawner damageNumberSpawner;

        protected Health health;
        protected Mover mover;
        protected FovChecker fovChecker;
        protected FovRenderer fovRenderer;
        protected InteractionDetector interactionDetector;
        protected MeleeAttacker meleeAttacker;
        protected ProjectileAttacker projectileAttacker;
        protected RaycastAttacker raycastAttacker;
        protected ChargeAttacker chargeAttacker;
        protected HybridAttacker hybridAttacker;
        protected Inventory inventory;
        protected Equipper equipper;
        protected KnockbackReceiver knockbackReceiver;

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
            interactionDetector = gameObject.AddComponent<InteractionDetector>();
            meleeAttacker = gameObject.AddComponent<MeleeAttacker>();
            projectileAttacker = gameObject.AddComponent<ProjectileAttacker>();
            raycastAttacker = gameObject.AddComponent<RaycastAttacker>();
            chargeAttacker = gameObject.AddComponent<ChargeAttacker>();
            hybridAttacker = gameObject.AddComponent<HybridAttacker>();
            inventory = gameObject.AddComponent<Inventory>();
            equipper = gameObject.AddComponent<Equipper>();
            knockbackReceiver = gameObject.AddComponent<KnockbackReceiver>();

            // GameObject fovObject = new GameObject("FovMesh");
            // fovObject.transform.parent = transform;
            // fovObject.transform.localPosition = Vector3.zero;
            // fovObject.transform.rotation = Quaternion.identity;
            // fovRenderer = fovObject.AddComponent<FovRenderer>();
            // fovRenderer.Chekcer = fovChecker;
        }

        /// <summary>
        /// 실제 스탯 적용
        /// </summary>
        protected virtual void InitStatus()
        {
            mover.BaseMoveSpeed = stats.moveSpeed;

            health.BaseMaxHp = stats.maxHp;
            health.CurrentHp = stats.maxHp;

            fovChecker.ViewDistance = stats.viewDistance;
            fovChecker.TargetMask = targetMask;
            fovChecker.ObstacleMask = obstacleMask;

            meleeAttacker.SetLayerMask(targetMask);
            projectileAttacker.SetLayerMask(targetMask);
            projectileAttacker.SetObstacleMask(obstacleMask);
            raycastAttacker.SetLayerMask(targetMask);
            raycastAttacker.SetObstacleMask(obstacleMask);
            raycastAttacker.Init(beamRenderer);
            chargeAttacker.Init(mover);
            chargeAttacker.SetLayerMask(targetMask);
            hybridAttacker.Init(meleeAttacker, projectileAttacker, raycastAttacker, chargeAttacker);

            inventory.BaseMaxWeight = stats.maxWeight;
            inventory.InitSlot(stats.inventoryCapacity);
            equipper.Init(health, mover, inventory, meleeAttacker, projectileAttacker, raycastAttacker, chargeAttacker, hybridAttacker);

            knockbackReceiver.KnockbackResistance = stats.knockbackResistance;
            knockbackReceiver.Init(health, mover);

            actorAnimator?.Init(mover, health, equipper, fovChecker);
            hitFlash?.Init(health);
            weaponAimer?.Init(equipper);
            attackEffectSpawner?.Init(equipper);
            hitStop?.Init(mover);
            meleeAttacker.SetHitStop(hitStop);

            healthBar?.SetTarget(health);
            damageNumberSpawner?.Init(health);
        }

        protected virtual void DieRoutine()
        {
            LootBox lootBox = Instantiate(lootBoxPrefab, transform.position, Quaternion.identity);
            lootBox.gameObject.name = $"{name}'s box";

            // (Enemy만 해당) 장착 장비도 같이 회수
            foreach (var equipment in equipper.GetEquippedItems())
            {
                inventory.AddItem(equipment);
            }

            inventory.MoveItemsTo(lootBox.Inventory);
        }
    }
}
