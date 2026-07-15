using Actors.Enemy.States;
using Actors.Player;
using Core.System;
using Core.System.FSM;
using Item.Data;
using Item.ItemObject;
using System.Diagnostics;
using UnityEngine;

namespace Actors.Enemy
{
    public class EnemyController : ActorController
    {
        [Header("Attack Range")]
        [SerializeField] private float attackRange = 1f;
        [SerializeField] private float traceDist = 20f;

        private StateMachine fsm;
        private EnemyContext context;

        public override void InitSettings()
        {
            base.InitSettings();
            health.OnDie += DieRoutine;
            SetupFSM();
        }
        
        private void SetupFSM()
        {
            context = new EnemyContext
            { 
                transform = transform,
                mover = mover,
                attacker = attacker,
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

        private void Update()
        {
            fovChecker.FindVisibleTargets();
            UpdateTarget();
            fsm.Update();
        }

        private void UpdateTarget()
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

        private float GetTargetDistance()
        {
            return Vector3.Distance(transform.position, context.target.position);
        }

        private void DieRoutine()
        {
            GameObject boxObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            boxObj.name = $"{name}'s box";
            boxObj.transform.position = transform.position;
            boxObj.transform.rotation = Quaternion.identity;
            boxObj.layer = LayerMask.NameToLayer("Interactable");

            LootBox lootBox = boxObj.AddComponent<LootBox>();
            inventory.MoveItemsTo(lootBox.Inventory);
        }
    }
}