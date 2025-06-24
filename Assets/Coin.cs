using UnityEngine;

public class Coin : MonoBehaviour, ICollectable
{
    public void Collect()
    {
        Debug.Log("Coin collected!");
        Destroy(gameObject);
    }
} 