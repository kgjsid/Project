using System;
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
        public Attacker attacker;

        public event Action OnStatsChanged;

        public void Init(Health health, Mover mover, Attacker attacker)
        {
            this.health = health;
            this.mover = mover;
            this.attacker = attacker;
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

            if (equipped.TryGetValue(EquipSlotType.PrimaryWeapon, out var data) && data is WeaponData weapon)
            {
                attacker.SetWeapon(weapon);
            }
            else
            {
                attacker.ClearWeapon();
            }

            OnStatsChanged?.Invoke();
        }
    }
}
