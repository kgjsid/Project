using UnityEngine;

using Actors.Enemy.States;
using Actors.Player;
using Core.System.FSM;
using Core.System;
using Item.Data;
using Item.ItemObject;

namespace Actors.Enemy
{
    public class EnemyController : ActorController
    {
        [Header("Attack TelegraphIndicator")]
        [SerializeField] protected TelegraphIndicator telegraphIndicator;

        [Header("Enemy Inventory")]
        [SerializeField] protected WeaponData startingWeapon;
        [SerializeField] protected LootEntry[] lootTable;

        protected StateMachine fsm;
        protected EnemyContext context;

        public override void InitSettings()
        {
            base.InitSettings();
            health.OnDie += DieRoutine;
            health.OnDamaged += HandleDamaged;
            SetupFSM();

            if (startingWeapon != null) equipper.Equip(startingWeapon);

            CheckLootTable();
        }

        protected virtual void SetupFSM()
        {
            context = new EnemyContext
            { 
                enemyController = this,
                transform = transform,
                mover = mover,
                equipper = equipper,
                attackRange = stats.attackRange,
                traceDist = stats.traceDist,
                telegraphIndicator = telegraphIndicator,
                knockbackReceiver = knockbackReceiver,
                telegraphDuration = stats.telegraphDuration,
                attackRecovery = stats.attackRecovery,
                deathDuration = 1f,
                // deathDuration = stats.deathDuration
            };

            var idleState = new IdleState(context);
            var traceState = new TraceState(context);
            var searchState = new SearchState(context);
            var telegraphState = new TelegraphState(context);
            var attackState = new AttackState(context);
            var dieState = new DieState(context);

            fsm = new StateMachine();

            // 사망 상태
            fsm.AddAnyTransition(dieState, () => health.IsDead());

            // 추격 상태
            fsm.AddTransition(idleState, traceState, () => context.target != null);
            fsm.AddTransition(traceState, idleState, () => context.target == null);

            // 추격 -> 탐색 상태
            fsm.AddTransition(traceState, searchState,
                () => context.target != null && !context.isTargetVisible);

            fsm.AddTransition(searchState, traceState,
                () => context.isTargetVisible);

            fsm.AddTransition(searchState, idleState,
                () => context.target == null);

            // 추격 -> 공격 준비(예고)상태
            fsm.AddTransition(traceState, telegraphState,
                () => context.target != null && GetTargetDistance() <= context.attackRange);

            fsm.AddTransition(telegraphState, attackState, () => telegraphState.IsFinished());
            
            fsm.AddTransition(attackState, traceState,
                () => attackState.IsFinished() && context.target != null);
            fsm.AddTransition(attackState, idleState,
                () => attackState.IsFinished() && context.target == null);

            fsm.SetState(idleState);
        }

        protected virtual void CheckLootTable()
        {
            if (lootTable == null) return;

            foreach(var entry in lootTable)
            {
                if (entry.item == null) continue;
                if (Random.value > entry.dropPercent) continue;

                int count = Random.Range(entry.minDropCount, entry.maxDropCount + 1);
                inventory.AddItem(entry.item, count);
            }
        }

        protected virtual void Update()
        {
            fovChecker.FindVisibleTargets();
            UpdateTarget();
            UpdateFacing();
            fsm.Update();
        }

        protected virtual void UpdateTarget()
        {
            if(context.target != null)
            {
                bool visible = fovChecker.VisibleTargets.Contains(context.target);
                context.isTargetVisible = visible;

                if(visible)
                {
                    context.lastTargetPosition = context.target.position;
                }

                if (GetTargetDistance() > context.traceDist)
                {
                    context.target = null;
                    context.isTargetVisible = false;
                }

                return;
            }

            context.isTargetVisible = false;
            foreach(var visible in fovChecker.VisibleTargets)
            {
                if (visible.TryGetComponent(out PlayerController player))
                {
                    context.target = player.transform;
                    context.isTargetVisible = true;
                    context.lastTargetPosition = player.transform.position;
                    return;
                }
            }
        }

        protected virtual float GetTargetDistance()
        {
            return Vector3.Distance(transform.position, context.target.position);
        }

        protected virtual void UpdateFacing()
        {
            if (context.target == null) return;

            Vector2 dir = (Vector2)context.target.position - (Vector2)transform.position;
            fovChecker.SetFacingDirection(dir);   // 시야는 계속 따라가도 됨

            // 조준은 텔레그래프/공격 중엔 고정
            if (!context.isAimLocked)
            {
                context.equipper.GetCurrentAttacker()?.SetAimDirection(dir);
            }
        }

        protected virtual void HandleDamaged(float damage, Vector2 hitDirection, float knockbackForce)
        {
            if (context.isAimLocked) return;
            if (context.target != null) return;

            if(hitDirection.sqrMagnitude > 0.0001f)
            {
                fovChecker.SetFacingDirection(-hitDirection);
            }
        }

        protected override void DieRoutine()
        {
            mover.Move(Vector2.zero);
            
            if (fovChecker != null && fovRenderer != null)
            {
                fovChecker.enabled = false;
                fovRenderer.gameObject.SetActive(false);
            }

            Collider2D collider = GetComponent<Collider2D>();
            if (collider != null) collider.enabled = false;
        }

        public void SpawnLootBox()
        {
            LootBox lootBox = Instantiate(lootBoxPrefab, transform.position, Quaternion.identity);
            lootBox.gameObject.name = $"{name}'s box";

            foreach(var equipment in equipper.GetEquippedItems())
            {
                inventory.AddItem(equipment);
            }
            inventory.MoveItemsTo(lootBox.Inventory);

            Destroy(gameObject);
        }
    }
}