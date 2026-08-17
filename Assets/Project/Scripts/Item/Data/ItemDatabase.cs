using System.Collections.Generic;
using UnityEngine;

namespace Item.Data
{
    [CreateAssetMenu(fileName = "ItemDatabase", menuName = "Items/Database")]
    public class ItemDatabase : ScriptableObject
    {
        [SerializeField] private List<ItemData> allItems = new List<ItemData>();

        private Dictionary<string, ItemData> lookup;

        public IReadOnlyList<ItemData> AllItems { get { return allItems; } }

        public ItemData GetById(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;

            if (lookup == null) BuildLookup();

            return lookup.TryGetValue(id, out ItemData result) ? result : null;
        }

        private void BuildLookup()
        {
            lookup = new Dictionary<string, ItemData>();

            foreach(ItemData item in AllItems)
            {
                if (item == null || string.IsNullOrEmpty(item.id)) continue;
                lookup[item.id] = item;
            }
        }

#if UNITY_EDITOR

        private void OnValidate()
        {
            lookup = null;

            HashSet<string> checkList = new HashSet<string>();

            foreach(ItemData item in allItems)
            {
                if (item == null)
                {
                    Debug.LogWarning($"[{name}] 비어있는 항목이 있습니다.", this);
                    continue;
                }

                if (string.IsNullOrEmpty(item.id))
                {
                    Debug.LogWarning($"[{name}] '{item.name}'의 id가 비어있습니다.", item);
                    continue;
                }

                if (!checkList.Add(item.id))
                {
                    Debug.LogError($"[{name}] id 중복: '{item.id}'", item);
                }
            }
        }

#endif
    }
}