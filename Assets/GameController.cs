using R3;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameController : MonoBehaviour,InputSystem_Actions.IUIActions
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerModel playerModel;
    [SerializeField] private CoinController coinController;

    private readonly CompositeDisposable _disposables = new();
    public static ReactiveProperty<bool> IsGamePaused = new(false);
    private InputSystem_Actions _actions;

    private void Awake()
    {
        if (playerController == null || playerModel == null || coinController == null)
        {
            return;
        }
         if(_actions == null)
            _actions = new InputSystem_Actions();

        _actions.Player.SetCallbacks(playerController);
        _actions.UI.SetCallbacks(this);
        _actions.Enable();

        playerModel.Initialize(playerController.OnMoveStream, playerController.OnJumpStream);
        playerController.Initialize();
        playerController.OnCoinCollect
            .Subscribe(_ =>
            {
                Debug.Log("Coin collected!");
                coinController.AddCoin();
            })
            .AddTo(_disposables);

        playerController.OnJumpStream
            .ThrottleFirst(TimeSpan.FromSeconds(0.25))
            .Subscribe(_ => Debug.Log("Player Jumped!"))
            .AddTo(_disposables);

        IsGamePaused
            .Subscribe(paused => {
               playerModel.HandlePause(paused);
            })
            .AddTo(_disposables);
    }

    private void OnDestroy()
    {
        _disposables.Dispose();
        _actions.Disable();
    }

    public void OnPause(InputAction.CallbackContext context)
    {   if(context.performed)
        IsGamePaused.Value = !IsGamePaused.Value;
    }
} 