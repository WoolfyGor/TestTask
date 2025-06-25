using R3;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameController : MonoBehaviour,InputSystem_Actions.IUIActions
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerModel playerModel;
    [SerializeField] private CoinController coinController;
    [SerializeField] private PauseController pauseController;

    private readonly CompositeDisposable _disposables = new();
    public static ReactiveProperty<bool> IsGamePaused = new(false);
    private InputSystem_Actions _actions;

    public static bool loadGame = false;

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
            .Subscribe(async _ =>
            {
                coinController.AddCoin();
                await SaveController.SaveAsync(playerController);
            })
            .AddTo(_disposables);

        playerController.OnJumpStream
            .ThrottleFirst(TimeSpan.FromSeconds(0.25))
            .Subscribe(_ => {})
            .AddTo(_disposables);

        IsGamePaused
            .Subscribe(async paused => {
               playerModel.HandlePause(paused);
               pauseController.SetPauseScreenVisible(paused);
               if (paused)
                   await SaveController.SaveAsync(playerController);
            })
            .AddTo(_disposables);


       
    }
    private void Start()
    {
        if (loadGame)
            SaveController.LoadSave(playerController,coinController);
    }
    private void OnDestroy()
    {
        _disposables.Dispose();
        _actions.Disable();
        IsGamePaused.Value = false;
        loadGame = false;
    }

    public void OnPause(InputAction.CallbackContext context)
    {   if(context.performed)
            HandlePause();
    }
    
    public void HandlePause(bool? value = null){
        IsGamePaused.Value = value!=null ? (bool)value:!IsGamePaused.Value;
    }
} 