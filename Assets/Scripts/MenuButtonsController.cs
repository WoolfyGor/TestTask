using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

public class MenuButtonsController : MonoBehaviour
{
    [SerializeField]
    private Button _continueButton, _newGameButton, _exitButton;
    [Inject]
    ISaveController saveController;

    async void Awake()
    {
        SetupButtons();
        bool result = await saveController.LoadAsync();
        if(_continueButton!=null)
            _continueButton.interactable = result;
    }

    void SetupButtons()
    {
        _continueButton?.onClick.AddListener(ContinueGame);
        _newGameButton?.onClick.AddListener(StartNewGame);
        _exitButton?.onClick.AddListener(ExitGame);
    }

    private void OnDisable()
    {
        _continueButton?.onClick.RemoveListener(ContinueGame);
        _newGameButton?.onClick.RemoveListener(StartNewGame);
        _exitButton?.onClick.RemoveListener(ExitGame);
    }
    void ContinueGame()
    {
        GameController.IsGameLoaded = true;
        SceneManager.LoadScene("NewGameScene");
    }

    void StartNewGame() => SceneManager.LoadScene("NewGameScene");
    void ExitGame() => Application.Quit();
}
