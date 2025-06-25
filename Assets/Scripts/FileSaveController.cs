using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Контроллер сохранения и загрузки данных игры.
/// </summary>
public class FileSaveController : MonoBehaviour,ISaveController
{
    /// <summary>
    /// Путь к файлу сохранения.
    /// </summary>
    private string SavePath => Path.Combine(Application.persistentDataPath, "save.json");

    /// <summary>
    /// Загруженные данные сохранения.
    /// </summary>
    public SaveData _loadedData;

    /// <summary>
    /// Асинхронно сохраняет данные игрока и коллекционных предметов.
    /// </summary>
    /// <param name="player">Контроллер игрока.</param>
    public async UniTask SaveAsync(PlayerController player)
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
    }

    /// <summary>
    /// Асинхронно загружает данные из файла сохранения.
    /// </summary>
    /// <returns>True, если загрузка успешна, иначе false.</returns>
    public async UniTask<bool> LoadAsync()
    {
        if (!File.Exists(SavePath))
            return false;

        string json;
        using (var reader = new StreamReader(SavePath))
        {
            json = await reader.ReadToEndAsync();
        }

        var data = JsonUtility.FromJson<SaveData>(json);
        if (data == null)
            return false;
        _loadedData = data;
       
        return true;
    }

    /// <summary>
    /// Применяет загруженные данные к игроку и монеткам.
    /// </summary>
    /// <param name="player">Контроллер игрока.</param>
    /// <param name="coins">Контроллер монет.</param>
    public void LoadSave(PlayerController player,CoinController coins){

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

    }

    /// <summary>
    /// Удаляет файл сохранения.
    /// </summary>
    public void ClearSave()
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

    void OnDestroy(){
        _loadedData = null;
    }
}

/// <summary>
/// Данные о коллекционном предмете для сохранения.
/// </summary>
[System.Serializable]
public class CollectableSaveData
{
    /// <summary>
    /// Уникальный идентификатор предмета.
    /// </summary>
    public int id;
    /// <summary>
    /// Был ли предмет собран.
    /// </summary>
    public bool isCollected;
}

/// <summary>
/// Данные для сохранения состояния игры.
/// </summary>
[System.Serializable]
public class SaveData
{
    /// <summary>
    /// Общее количество собранных предметов.
    /// </summary>
    public int totalCollected;
    /// <summary>
    /// Список коллекционных предметов.
    /// </summary>
    public List<CollectableSaveData> collectables = new List<CollectableSaveData>();
    /// <summary>
    /// Позиция игрока.
    /// </summary>
    public Vector3 playerPosition;
}