using System.IO;
using UnityEditor;
using UnityEngine;

public static class SaveDebugMenu
{
    [MenuItem("Tools/Save/Delete Save File")]
    private static void DeleteSave()
    {
        string path = Path.Combine(Application.persistentDataPath, "save.json");

        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log($"세이브 삭제됨: {path}");
        }
        else
        {
            Debug.Log("세이브 파일이 없습니다.");
        }
    }

    [MenuItem("Tools/Save/Open Save Folder")]
    private static void OpenFolder()
    {
        EditorUtility.RevealInFinder(Application.persistentDataPath);
    }
}