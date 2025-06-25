using TMPro;
using UnityEngine;

public class CoinController : MonoBehaviour
{
    [SerializeField] private TMP_Text coinText;
    private int coins;

    private void Start()
    {
        UpdateText();
    }

    /// <summary>
    /// Добавляет монеты игроку.
    /// </summary>
    /// <param name="amount">Количество добавляемых монет (по умолчанию 1).</param>
    public void AddCoin(int amount = 1)
    {
        coins += amount;
        UpdateText();
    }

    /// <summary>
    /// Устанавливает текущее количество монет.
    /// </summary>
    /// <param name="count">Новое количество монет.</param>
    public void SetCoins(int count)
    {
        coins = count;
        UpdateText();
    }

    /// <summary>
    /// Возвращает текущее количество монет.
    /// </summary>
    /// <returns>Текущее количество монет.</returns>
    public int GetCoins() => coins;

    private void UpdateText()
    {
        if (coinText != null)
            coinText.text = coins.ToString();
    }
} 