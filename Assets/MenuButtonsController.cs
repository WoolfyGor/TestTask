using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuButtonsController : MonoBehaviour
{
    [SerializeField]
    private Button _continueButton, _newGameButton, _exitButton;


    async void Awake()
    {
        SetupButtons();
        bool result = await SaveController.LoadAsync();
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
        GameController.loadGame = true;
        SceneManager.LoadScene("NewGameScene");
    }

    void StartNewGame() => SceneManager.LoadScene("NewGameScene");
    void ExitGame() => Application.Quit();
}
