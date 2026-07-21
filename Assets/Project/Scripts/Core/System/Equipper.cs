using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

using Item.Data;

namespace Core.System
{
    public class Equipper : MonoBehaviour
    {
        // SlotType 별 장비된 아이템을 저장할 딕셔너리
        private Dictionary<EquipSlotType, EquipmentData> equipped = new Dictionary<EquipSlotType, EquipmentData>();

        public Health health;
        public Mover mover;
        public MeleeAttacker meleeAttacker;
        public ProjectileAttacker projectileAttacker;

        public event Action OnStatsChanged;

        private IAttacker currentAttacker;

        public IAttacker GetCurrentAttacker()
        {
            return currentAttacker;
        }

        public void Init(Health health, Mover mover, MeleeAttacker meleeAttacker, ProjectileAttacker projectileAttacker)
        {
            this.health = health;
            this.mover = mover;
            this.meleeAttacker = meleeAttacker;
            this.projectileAttacker = projectileAttacker;
        }

        public void Equip(EquipmentData data)
        {
            equipped[data.equipSlot] = data;
            RecalculateStats();
        }

        public void Unequip(EquipSlotType slot)
        {
            equipped.Remove(slot);
            RecalculateStats();
        }

        public List<EquipmentData> GetEquippedItems()
        {
            return equipped.Values.ToList();
        }

        private void RecalculateStats()
        {
            float defenseBonus = 0f;
            float moveSpeedBonus = 0f;

            foreach(var item in equipped.Values)
            {
                if(item is ArmorData armor)
                {
                    defenseBonus += armor.defense;
                    moveSpeedBonus += armor.moveSpeedMod;
                }
            }

            health.BonusMaxHp = defenseBonus;
            mover.BonusMoveSpeed = moveSpeedBonus;

            ApplyWeapon();

            OnStatsChanged?.Invoke();
        }

        private void ApplyWeapon()
        {
            if (equipped.TryGetValue(EquipSlotType.PrimaryWeapon, out var data) && data is WeaponData weapon)
            {
                if (weapon.attackType == AttackType.Melee)
                {
                    meleeAttacker.SetWeapon(weapon);
                    projectileAttacker.ClearWeapon();
                    currentAttacker = meleeAttacker;
                }
                else
                {
                    projectileAttacker.SetWeapon(weapon);
                    meleeAttacker.ClearWeapon();
                    currentAttacker = projectileAttacker;
                }
            }
            else
            {
                meleeAttacker.ClearWeapon();
                projectileAttacker.ClearWeapon();
                currentAttacker = null;
            }
        }
    }
}
