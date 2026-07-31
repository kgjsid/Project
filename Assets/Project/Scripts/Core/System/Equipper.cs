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
        public Inventory inventory;
        public MeleeAttacker meleeAttacker;
        public ProjectileAttacker projectileAttacker;

        public event Action OnStatsChanged;

        private IAttacker currentAttacker;

        // 이동 속도 패널티
        private float overweightPenalty = 2f;

        public IAttacker GetCurrentAttacker()
        {
            return currentAttacker;
        }

        public void Init(Health health, Mover mover, Inventory inventory, MeleeAttacker meleeAttacker, ProjectileAttacker projectileAttacker)
        {
            this.health = health;
            this.mover = mover;
            this.inventory = inventory;
            this.meleeAttacker = meleeAttacker;
            this.projectileAttacker = projectileAttacker;

            inventory.OnInventoryChanged += RecalculateStats;
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

        public WeaponData GetEquippedWeapon()
        {
            if (equipped.TryGetValue(EquipSlotType.PrimaryWeapon, out var data) && data is WeaponData weapon)
            {
                return weapon;
            }
            return null;
        }

        public List<EquipmentData> GetEquippedItems()
        {
            return equipped.Values.ToList();
        }

        private void RecalculateStats()
        {
            float defenseBonus = 0f;
            float moveSpeedBonus = 0f;
            float weightBonus = 0f;

            foreach(var item in equipped.Values)
            {
                if(item is ArmorData armor)
                {
                    defenseBonus += armor.defense;
                    moveSpeedBonus += armor.moveSpeedMod;
                    weightBonus += armor.weightBonus;
                }
            }

            health.BonusMaxHp = defenseBonus;
            inventory.BonusMaxWeight = weightBonus;

            if (inventory.IsOverweight)
            {
                moveSpeedBonus -= overweightPenalty;
            }
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

        private void OnDisable()
        {
            if (inventory != null)
            {
                inventory.OnInventoryChanged -= RecalculateStats;
            }
        }
    }
}
