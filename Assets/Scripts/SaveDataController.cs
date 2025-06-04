public static class SaveDataController
{
    private static JsonSaveHandler[] saveFiles = new JsonSaveHandler[3];
    public static void Save<T>(T target, int index = 0) where T : SaveData
    {
        saveFiles[index].Save(target);
    }
    public static void Load<T>(ref T target, int index = 0) where T : SaveData, new()
    {
        saveFiles[index].Load(ref target);
    }
}
