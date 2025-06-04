using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class JsonSaveHandler
{
    public static string basePath
    {
        get
        {
            return Path.Combine(Application.dataPath, "SaveData");
        }
    }
    public void Save<T>(T target) where T : SaveData
    {
        Directory.CreateDirectory(basePath);
        
        string saveFilePath = GetFilePath(basePath);
        string jsonString = JsonUtility.ToJson(target);

        if (IsFileEmpty(jsonString)) return;
        
        File.WriteAllText(saveFilePath, jsonString);
        IsFileAccessible(saveFilePath);
    }
    // 지정제한자로 new()를 추가하는 이유는,
    // JsonUtility가 매개변수가 없는 생성자를 필요로 하기 때문
    public void Load<T>(ref T target) where T : SaveData, new()
    {
        string filePath = GetFilePath(target.GetType().ToString());
        string jsonString = File.ReadAllText(filePath);

        if(!IsFileAccessible(filePath)) return;
        if (IsFileEmpty(jsonString)) return;

        target = JsonUtility.FromJson<T>(jsonString);
    }
    private bool IsFileEmpty(string jsonString)
    {
        if(string.IsNullOrEmpty(jsonString))
        {
            Debug.LogError("Save File Empty");
            return true;
        }
        return false;
    }
    private bool IsFileAccessible(string filePath)
    {
        if(File.Exists(filePath))
        {
            Debug.Log($"Save File Exists at path: {filePath}");
            return true;
        }
        else
        {
            Debug.LogError($"cannot access to save file at path: {filePath}");
            return false;
        }
    }
    protected static string GetFilePath(string fileName)
    {
        return Path.Combine(basePath, $"{fileName}.json");
    }
}
