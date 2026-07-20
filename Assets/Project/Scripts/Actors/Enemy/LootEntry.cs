using System;
using UnityEngine;

using Item.Data;

namespace Actors.Enemy
{
    [Serializable]
    public class LootEntry
    {
        public ItemData item;
        public int minDropCount = 1;
        public int maxDropCount = 1;
        [Range(0f, 1f)] public float dropPercent = 1f;
    }
}