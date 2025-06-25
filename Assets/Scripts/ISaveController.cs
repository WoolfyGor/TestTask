using Cysharp.Threading.Tasks;

public interface ISaveController
{
    public UniTask SaveAsync(PlayerController controller,PlayerModel model);
    public UniTask<bool> LoadAsync();
    public void LoadSave(PlayerController player, PlayerModel model, CoinController coins);
    public void ClearSave();
} 