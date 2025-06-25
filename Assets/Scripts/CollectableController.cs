using System.Collections.Generic;
using UnityEngine;

public class CollectableController : MonoBehaviour
{
    public static CollectableController Instance { get; private set; }
    private static readonly List<ICollectable> collectables = new List<ICollectable>();
    private static readonly HashSet<int> ids = new HashSet<int>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

    }

    /// <summary>
    /// Регистрирует новый коллекционный объект.
    /// </summary>
    /// <param name="collectable">Коллекционный объект для регистрации.</param>
    /// <returns>True, если регистрация успешна, иначе false.</returns>
    public static bool RegisterCollectable(ICollectable collectable)
    {
        if (collectable.ID <= 0)
        {
            Debug.LogError($"Collectable ID некорректен (<=0): {collectable.ID}", collectable as Object);
            return false;
        }
        if (!ids.Add(collectable.ID))
        {
            Debug.LogError($"Дублирующийся Collectable ID: {collectable.ID}", collectable as Object);
            return false;
        }
        collectables.Add(collectable);
        return true;
    }

    void OnDestroy(){

        Instance = null;
        collectables.Clear();
        ids.Clear();
    }

    /// <summary>
    /// Возвращает список всех коллекционных объектов.
    /// </summary>
    /// <returns>Список всех коллекционных объектов.</returns>
    public static IReadOnlyList<ICollectable> GetAllCollectables() => collectables.AsReadOnly();
} 