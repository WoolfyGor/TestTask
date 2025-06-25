using UnityEngine;

public class Coin : MonoBehaviour, ICollectable
{
    [SerializeField] private int id;
    public int ID => id;
    public bool Collected { get; private set; }

    private void Awake()
    {
        if (id <= 0)
        {
            Debug.LogError($"Coin ID некорректен (<=0): {id}", this);
        }
        CollectableController.RegisterCollectable(this);
    }
    public void Collect()
    {
        if (Collected) return;
        Collected = true;
        Debug.Log($"Coin collected! ID: {id}");
        gameObject.SetActive(false);
    }
    /// <summary>
    /// Меняет статус монетки и её видимость на сцене
    /// </summary>
    /// <param name="collected">Была собрана?</param>
    public void SetCollected(bool collected)
    {
        Collected = collected;
        gameObject.SetActive(!collected);
    }
} 