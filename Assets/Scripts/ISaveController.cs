using Cysharp.Threading.Tasks;

public interface ISaveController
{
    public UniTask SaveAsync(PlayerController player);
    public UniTask<bool> LoadAsync();
    public void LoadSave(PlayerController player, CoinController coins);
    public void ClearSave();
} 