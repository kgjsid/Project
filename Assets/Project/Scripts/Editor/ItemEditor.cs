using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEditor;
using UnityEngine;

using Item.Data;

public static class ItemTableImporter
{
    private const string TABLE_DIR = "Assets/Project/Data/Tables";
    private const string OUTPUT_DIR = "Assets/Project/Data/Item/Generated";

    private static Dictionary<string, Sprite> spriteCache;

    [MenuItem("Tools/Item/Import Item Tables")]
    public static void Import()
    {
        spriteCache = null; 
        EnsureFolder(OUTPUT_DIR);

        int created = 0, updated = 0;

        ImportSheet<WeaponData>("Weapons.tsv", ApplyWeapon, "Weapon", ref created, ref updated);
        ImportSheet<ArmorData>("Armors.tsv", ApplyArmor, "Armor", ref created, ref updated);
        ImportSheet<ConsumableData>("Consumables.tsv", ApplyConsumable, "Consumable", ref created, ref updated);
        ImportSheet<MaterialData>("Materials.tsv", ApplyCommonOnly, "Material", ref created, ref updated);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"아이템 임포트 완료 — 생성 {created}, 갱신 {updated}");
    }

    private static void ImportSheet<T>(string fileName, Action<T, Row> apply, string subFolder, ref int created, ref int updated)
        where T : ItemData
    {
        string path = Path.Combine(TABLE_DIR, fileName);

        if (!File.Exists(path))
        {
            Debug.LogWarning($"테이블 파일 없음: {path}");
            return;
        }

        List<Row> rows = ParseTsv(path);

        foreach (Row row in rows)
        {
            string id = row.GetString("id");
            if (string.IsNullOrEmpty(id))
            {
                Debug.LogWarning($"{fileName}: id가 빈 행을 건너뜁니다.");
                continue;
            }

            string folder = Path.Combine(OUTPUT_DIR, subFolder).Replace("\\", "/");
            if (subFolder == "Armor")
            {
                string slot = row.GetString("equipSlot");
                if (!string.IsNullOrEmpty(slot))
                    folder = $"{folder}/{slot}";
            }

            EnsureFolder(folder);

            string assetPath = $"{folder}/{id}.asset";
            T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);

            bool isNew = asset == null;
            if (isNew)
            {
                asset = ScriptableObject.CreateInstance<T>();
                AssetDatabase.CreateAsset(asset, assetPath);
                created++;
            }
            else
            {
                updated++;
            }

            ApplyCommon(asset, row);
            apply(asset, row);

            EditorUtility.SetDirty(asset);
        }
    }

    // 필드 적용

    private static void ApplyCommon(ItemData item, Row row)
    {
        item.id = row.GetString("id");
        item.itemName = row.GetString("itemName");
        item.itemType = row.GetEnum("itemType", ItemType.Material);
        item.maxStack = row.GetInt("maxStack", 1);
        item.sellPrice = row.GetInt("sellPrice");
        item.weight = row.GetFloat("weight");
        item.description = row.GetString("description");
        item.icon = FindSprite(row.GetString("iconName"));
    }

    private static void ApplyCommonOnly(MaterialData item, Row row) { }

    private static void ApplyWeapon(WeaponData w, Row row)
    {
        w.equipSlot = row.GetEnum("equipSlot", EquipSlotType.MainHand);
        w.attackType = row.GetEnum("attackType", AttackType.Melee);
        w.damage = row.GetFloat("damage");
        w.range = row.GetFloat("range");
        w.swingAngle = row.GetFloat("swingAngle", 100f);
        w.attackSpeed = row.GetFloat("attackSpeed", 1f);
        w.knockbackForce = row.GetFloat("knockbackForce");
        w.durability = row.GetInt("durability");
        w.effectColor = ParseColor(row.GetString("effectColorHex"));
        w.effectPrefab = FindAsset<GameObject>(row.GetString("effectPrefabName"));
        w.telegraphSprite = FindSprite(row.GetString("telegraphSpriteName"));
        w.projectileSpeed = row.GetFloat("projectileSpeed");
        GameObject proj = FindAsset<GameObject>(row.GetString("projectilePrefabName"));
        w.projectilePrefab = proj != null ? proj.GetComponent<Core.System.Projectile>() : null;
    }

    private static void ApplyArmor(ArmorData a, Row row)
    {
        a.equipSlot = row.GetEnum("equipSlot", EquipSlotType.Body);
        a.defense = row.GetInt("defense");
        a.moveSpeedMod = row.GetFloat("moveSpeedMod");
        a.weightBonus = row.GetFloat("weightBonus");
        a.maxHpBonus = row.GetFloat("maxHpBonus");
        a.knockbackResistBonus = row.GetFloat("knockbackResistBonus");
        a.durability = row.GetInt("durability");
    }

    private static void ApplyConsumable(ConsumableData c, Row row)
    {
        c.effectType = row.GetEnum("effectType", ConsumableEffectType.Heal);
        c.effectAmount = row.GetFloat("effectAmount");
    }

    // 유틸

    private static T FindAsset<T>(string assetName) where T : UnityEngine.Object
    {
        if (string.IsNullOrEmpty(assetName)) return null;

        string filter = $"{assetName} t:{typeof(T).Name}";
        string[] guids = AssetDatabase.FindAssets(filter);

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            T asset = AssetDatabase.LoadAssetAtPath<T>(path);

            // 이름이 정확히 일치하는 것만 (부분 검색이라 유사 이름이 섞임)
            if (asset != null && asset.name == assetName) return asset;
        }

        Debug.LogWarning($"에셋을 찾지 못함: '{assetName}' ({typeof(T).Name})");
        return null;
    }

    private static Color ParseColor(string hex)
    {
        if (string.IsNullOrEmpty(hex)) return Color.white;
        return ColorUtility.TryParseHtmlString(hex, out Color c) ? c : Color.white;
    }

    private static void EnsureFolder(string path)
    {
        if (AssetDatabase.IsValidFolder(path)) return;

        string parent = Path.GetDirectoryName(path).Replace("\\", "/");
        string leaf = Path.GetFileName(path);

        EnsureFolder(parent);
        AssetDatabase.CreateFolder(parent, leaf);
    }

    // TSV 파싱

    private class Row
    {
        private readonly Dictionary<string, string> values = new Dictionary<string, string>();

        public Row(string[] header, string[] cells)
        {
            for (int i = 0; i < header.Length && i < cells.Length; i++)
                values[header[i].Trim()] = cells[i].Trim();
        }

        public string GetString(string key)
        {
            return values.TryGetValue(key, out string v) ? v : string.Empty;
        }

        public int GetInt(string key, int fallback = 0)
        {
            return int.TryParse(GetString(key), out int v) ? v : fallback;
        }

        public float GetFloat(string key, float fallback = 0f)
        {
            return float.TryParse(GetString(key), NumberStyles.Float, CultureInfo.InvariantCulture, out float v)
                ? v : fallback;
        }

        public TEnum GetEnum<TEnum>(string key, TEnum fallback) where TEnum : struct
        {
            string raw = GetString(key);
            return Enum.TryParse(raw, true, out TEnum v) ? v : fallback;
        }
    }

    private static List<Row> ParseTsv(string path)
    {
        string[] lines = File.ReadAllLines(path, System.Text.Encoding.UTF8);
        List<Row> result = new List<Row>();

        if (lines.Length < 2) return result;

        string[] header = lines[0].Split('\t');

        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;
            result.Add(new Row(header, lines[i].Split('\t')));
        }

        return result;
    }

    private static void BuildSpriteCache()
    {
        spriteCache = new Dictionary<string, Sprite>();

        string[] guids = AssetDatabase.FindAssets("t:Texture2D");

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);

            // Multiple 모드: 하위 에셋으로 존재
            foreach (UnityEngine.Object obj in AssetDatabase.LoadAllAssetRepresentationsAtPath(path))
            {
                if (obj is Sprite sub && !spriteCache.ContainsKey(sub.name))
                    spriteCache[sub.name] = sub;
            }

            // Single 모드: 메인 에셋이 곧 스프라이트
            Sprite main = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (main != null && !spriteCache.ContainsKey(main.name))
                spriteCache[main.name] = main;
        }
    }

    private static Sprite FindSprite(string spriteName)
    {
        if (string.IsNullOrEmpty(spriteName)) return null;

        if (spriteCache == null) BuildSpriteCache();

        if (spriteCache.TryGetValue(spriteName, out Sprite sprite)) return sprite;

        Debug.LogWarning($"스프라이트를 찾지 못함: '{spriteName}'");
        return null;
    }
}