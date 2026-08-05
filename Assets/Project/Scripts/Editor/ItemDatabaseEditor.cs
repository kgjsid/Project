using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

using Item.Data;

namespace ItemEditor
{
    [CustomEditor(typeof(ItemDatabase))]
    public class ItemDatabaseEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.Space();

            if (GUILayout.Button("프로젝트에서 모든 ItemData 찾아 채우기", GUILayout.Height(30)))
            {
                RefreshAll((ItemDatabase)target);
            }
        }

        private void RefreshAll(ItemDatabase database)
        {
            string[] guids = AssetDatabase.FindAssets("t:ItemData");
            List<ItemData> found = new List<ItemData>();

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                ItemData item = AssetDatabase.LoadAssetAtPath<ItemData>(path);

                if (item != null) found.Add(item);
            }

            SerializedObject so = new SerializedObject(database);
            SerializedProperty listProp = so.FindProperty("allItems");

            listProp.ClearArray();
            for (int i = 0; i < found.Count; i++)
            {
                listProp.InsertArrayElementAtIndex(i);
                listProp.GetArrayElementAtIndex(i).objectReferenceValue = found[i];
            }

            so.ApplyModifiedProperties();
            EditorUtility.SetDirty(database);
            AssetDatabase.SaveAssets();

            Debug.Log($"ItemDatabase: {found.Count}개 등록 완료", database);
        }
    }
}