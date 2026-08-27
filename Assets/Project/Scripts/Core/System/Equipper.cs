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
        public RaycastAttacker raycastAttacker;

        public event Action OnStatsChanged;

        private IAttacker currentAttacker;

        // 이동 속도 패널티
        private float overweightPenalty = 2f;

        public IAttacker GetCurrentAttacker()
        {
            return currentAttacker;
        }

        public void Init(Health health, Mover mover, Inventory inventory, MeleeAttacker meleeAttacker, ProjectileAttacker projectileAttacker, RaycastAttacker raycastAttacker)
        {
            this.health = health;
            this.mover = mover;
            this.inventory = inventory;
            this.meleeAttacker = meleeAttacker;
            this.projectileAttacker = projectileAttacker;
            this.raycastAttacker = raycastAttacker;

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
            if (equipped.TryGetValue(EquipSlotType.MainHand, out var data) && data is WeaponData weapon)
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

            if (equipped.ContainsKey(EquipSlotType.MainHand))
                ApplyWeapon(equipped[EquipSlotType.MainHand] as WeaponData);

            OnStatsChanged?.Invoke();
        }

        private void ApplyWeapon(WeaponData weapon)
        {
            if (weapon == null)
            {
                meleeAttacker.ClearWeapon();
                projectileAttacker.ClearWeapon();
                raycastAttacker.ClearWeapon();
                currentAttacker = null;

                return;
            }

            bool hasMelee = weapon.attackType.HasFlag(AttackType.Melee);
            bool hasProjectile = weapon.attackType.HasFlag(AttackType.Projectile);
            bool hasRaycast = weapon.attackType.HasFlag(AttackType.Raycast);

            if (hasMelee) meleeAttacker.SetWeapon(weapon);
            else meleeAttacker.ClearWeapon();

            if (hasProjectile) projectileAttacker.SetWeapon(weapon);
            else projectileAttacker.ClearWeapon();

            if (hasRaycast) raycastAttacker.SetWeapon(weapon);
            else raycastAttacker.ClearWeapon();

            if (hasMelee) currentAttacker = meleeAttacker;
            else if (hasProjectile) currentAttacker = projectileAttacker;
            else if (hasRaycast) currentAttacker = raycastAttacker;
            else currentAttacker = null;
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
