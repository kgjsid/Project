using UnityEngine;

using Actors.Enemy.States;
using Actors.Player;
using Core.System.FSM;
using Item.Data;
using Item.ItemObject;

namespace Actors.Enemy
{
    public class EnemyController : ActorController
    {
        [Header("Attack Range")]
        [SerializeField] protected float attackRange = 1f;
        [SerializeField] protected float traceDist = 20f;

        [Header("Enemy Inventory")]
        [SerializeField] protected WeaponData startingWeapon;
        [SerializeField] protected LootEntry[] lootTable;

        protected StateMachine fsm;
        protected EnemyContext context;

        public override void InitSettings()
        {
            base.InitSettings();
            health.OnDie += DieRoutine;
            SetupFSM();

            if (startingWeapon != null) equipper.Equip(startingWeapon);

            CheckLootTable();
        }

        protected virtual void SetupFSM()
        {
            context = new EnemyContext
            { 
                transform = transform,
                mover = mover,
                equipper = equipper,
                attackRange = attackRange,
                traceDist = traceDist
            };

            var idleState = new IdleState(context);
            var traceState = new TraceState(context);
            var attackState = new AttackState(context);
            var dieState = new DieState(context);

            fsm = new StateMachine();

            fsm.AddAnyTransition(dieState, () => health.IsDead());

            fsm.AddTransition(idleState, traceState, () => context.target != null);
            fsm.AddTransition(traceState, idleState, () => context.target == null);
            fsm.AddTransition(traceState, attackState, () => context.target != null && GetTargetDistance() <= context.attackRange);
            fsm.AddTransition(attackState, traceState, () => context.target != null && GetTargetDistance() > context.attackRange);
            fsm.AddTransition(attackState, idleState, () => context.target == null);

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
                if(GetTargetDistance() > context.traceDist)
                {
                    context.target = null;
                }

                return;
            }

            foreach(var visible in fovChecker.VisibleTargets)
            {
                if (visible.TryGetComponent(out PlayerController player))
                {
                    context.target = player.transform;
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
            if (context.target != null)
            {
                Vector2 dir = (Vector2)context.target.position - (Vector2)transform.position;
                fovChecker.SetFacingDirection(dir);
                context.equipper.GetCurrentAttacker()?.SetAimDirection(dir);
            }
        }

        protected override void DieRoutine()
        {
            base.DieRoutine();

            mover.Move(Vector2.zero);
            enabled = false;
            if (fovChecker != null && fovRenderer != null)
            {
                fovChecker.enabled = false;
                fovRenderer.gameObject.SetActive(false);
            }
        }
    }
}