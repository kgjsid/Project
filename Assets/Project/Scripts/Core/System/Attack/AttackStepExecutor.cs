using UnityEngine;

using Actors.Enemy;
using Item.Data;

namespace Core.System.Attack
{
    public static class AttackStepExecutor
    {
        public static void SetupPattern(EnemyContext context, WeaponData weapon, AttackStep[] steps)
        {
            bool needMelee = false, needProjectile = false, needCharge = false;

            for(int stepIndex = 0; stepIndex < steps.Length; stepIndex++)
            {
                switch(steps[stepIndex].type)
                {
                    case AttackStepType.Melee: 
                    case AttackStepType.Shockwave:
                        needMelee = true; 
                        break;
                    case AttackStepType.Projectile:
                        needProjectile = true;
                        break;
                    case AttackStepType.Charge:
                        needCharge = true;
                        break;
                }
            }

            if (needMelee && weapon.attackType.HasFlag(AttackType.Melee))
                context.equipper.GetMeleeAttacker().SetWeapon(weapon);
            if (needProjectile && weapon.attackType.HasFlag(AttackType.Projectile))
                context.equipper.GetProjectileAttacker().SetWeapon(weapon);
            if (needCharge && weapon.attackType.HasFlag(AttackType.Charge))
                context.equipper.GetChargeAttacker().SetWeapon(weapon);
        }

        public static void Execute(EnemyContext context, in AttackStep step, Vector2 aim)
        {
            switch (step.type)
            {
                case AttackStepType.Melee:
                    ExecuteAttack(context.equipper.GetMeleeAttacker(), aim);
                    break;
                case AttackStepType.Projectile:
                    ExecuteAttack(context.equipper.GetProjectileAttacker(), aim);
                    break;
                case AttackStepType.Charge:
                    ExecuteAttack(context.equipper.GetChargeAttacker(), aim);
                    break;
                case AttackStepType.Shockwave:
                    ExecuteAttack(context.equipper.GetMeleeAttacker(), aim);
                    break;
                case AttackStepType.Wait:
                    break;
            }
        }

        private static void ExecuteAttack(IAttacker attacker, Vector2 aim)
        {
            if (attacker == null) return;
            attacker.SetAimDirection(aim);
            attacker.ForceAttack();
        }
    }
}