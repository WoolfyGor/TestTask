using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class SaveController : MonoBehaviour
{
    private static string SavePath => Path.Combine(Application.persistentDataPath, "save.json");

    public static SaveData _loadedData;
    public static async UniTask SaveAsync(PlayerController player)
    {
        var all = CollectableController.GetAllCollectables();
        var data = new SaveData();
        foreach (var c in all)
        {
            data.collectables.Add(new CollectableSaveData { id = c.ID, isCollected = c.Collected });
            if (c.Collected) data.totalCollected++;
        }
        if (player != null)
            data.playerPosition = player.transform.position;
        string json = JsonUtility.ToJson(data, true);
        using (var writer = new StreamWriter(SavePath, false))
        {
            await writer.WriteAsync(json);
        }
        Debug.Log($"[SaveController] Сохранение завершено. Путь: {SavePath}");
    }

    public static async UniTask<bool> LoadAsync()
    {
        if (!File.Exists(SavePath))
        {
            Debug.Log("[SaveController] Файл сохранения не найден.");
            return false;
        }
        string json;
        using (var reader = new StreamReader(SavePath))
        {
            json = await reader.ReadToEndAsync();
        }
        var data = JsonUtility.FromJson<SaveData>(json);
        if (data == null)
        {
            Debug.LogError("[SaveController] Ошибка чтения файла сохранения.");
            return false;
        }
        _loadedData = data;
        
        Debug.Log($"[SaveController] Загрузка завершена. Путь: {SavePath}");
        return true;
    }

    public static void LoadSave(PlayerController player,CoinController coins){
        var all = CollectableController.GetAllCollectables();
        var dict = new Dictionary<int, bool>();
        foreach (var c in _loadedData.collectables)
            dict[c.id] = c.isCollected;
        foreach (var c in all)
        {
            if (dict.TryGetValue(c.ID, out var collected))
                c.SetCollected(collected);
        }

        if (player != null)
            player.transform.position = _loadedData.playerPosition;
        
        if (coins != null)
            coins.SetCoins(_loadedData.totalCollected);

         Debug.Log($"[SaveController] Загрузил сохранение.");
    }

    public static void ClearSave()
    {
        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
            Debug.Log($"[SaveController] Сохранение удалено. Путь: {SavePath}");
        }
        else
        {
            Debug.Log("[SaveController] Нет файла для удаления.");
        }
    }
}

[System.Serializable]
public class CollectableSaveData
{
    public int id;
    public bool isCollected;
}

[System.Serializable]
public class SaveData
{
    public int totalCollected;
    public List<CollectableSaveData> collectables = new List<CollectableSaveData>();
    public Vector3 playerPosition;
} 