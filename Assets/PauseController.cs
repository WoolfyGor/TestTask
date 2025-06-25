using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class PauseController : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private float _fadeDuration = 0.3f;
    [SerializeField] private GameController _gameController;


    void Awake(){
        if (_canvasGroup == null) _canvasGroup = GetComponent<CanvasGroup>();
    }
    public void SetPauseScreenVisible(bool visible,bool instant = false)
    {
        if (!visible)
        {
            _canvasGroup?.DOFade(0f, instant?0f:_fadeDuration).OnComplete(() => {
                if (_canvasGroup != null) _canvasGroup.interactable = false;
                if (_canvasGroup != null) _canvasGroup.blocksRaycasts = false;
            }).Play();
        }
        else
        {
             if (_canvasGroup != null) _canvasGroup.interactable = true;
             if (_canvasGroup != null) _canvasGroup.blocksRaycasts = true;
             _canvasGroup?.DOFade(1f, instant ? 0f : _fadeDuration).Play();
        }
    }

    public void GoToMenuScene()
    {
        SceneManager.LoadScene("MenuScene");
    }

    public void CallGameControllerPause()
    {
        if (_gameController != null)
            _gameController.HandlePause();
    }
}
