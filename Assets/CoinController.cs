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

    public void AddCoin(int amount = 1)
    {
        coins += amount;
        UpdateText();
    }

    public void SetCoins(int count)
    {
        coins = count;
        UpdateText();
    }

    public int GetCoins() => coins;


    private void UpdateText()
    {
        if (coinText != null)
            coinText.text = coins.ToString();
    }
} 