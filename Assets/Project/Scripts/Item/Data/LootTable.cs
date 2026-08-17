using System;
using System.Collections.Generic;
using UnityEngine;

using Item.Data;

namespace Item.Data
{
    [Serializable]
    public class LootEntry
    {
        public ItemData item;
        public int weight = 1;
        public int minCount = 1;
        public int maxCount = 1;
    }

    [CreateAssetMenu(fileName = "NewLootTable", menuName = "Item/LootTable")]
    public class LootTable : ScriptableObject
    {
        [SerializeField] private List<LootEntry> entries = new List<LootEntry>();

        [SerializeField] private int minRolls = 2;
        [SerializeField] private int maxRolls = 4;

        public List<(ItemData item, int count)> Roll()
        {
            var result = new List<(ItemData, int)>();

            if (entries == null || entries.Count == 0) return result;

            int totalWeight = 0;
            foreach (var e in entries)
            {
                if (e.item == null) continue;
                totalWeight += Mathf.Max(0, e.weight);
            }

            if (totalWeight <= 0) return result;

            int rolls = UnityEngine.Random.Range(minRolls, maxRolls + 1);

            for (int i = 0; i < rolls; i++)
            {
                LootEntry picked = PickWeighted(totalWeight);
                if (picked == null) continue;

                int count = UnityEngine.Random.Range(picked.minCount, picked.maxCount + 1);
                if (count <= 0) continue;

                result.Add((picked.item, count));
            }

            return result;
        }
        
        private LootEntry PickWeighted(int totalWeight)
        {
            int roll = UnityEngine.Random.Range(0, totalWeight);
            int cumulative = 0;

            foreach (var e in entries)
            {
                if (e.item == null) continue;

                cumulative += Mathf.Max(0, e.weight);
                if (roll < cumulative) return e;
            }

            return null;
        }
    }
}