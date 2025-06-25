using R3;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class GameController : MonoBehaviour,InputSystem_Actions.IUIActions
{

    [SerializeField] private PlayerController playerController;

    [SerializeField] private PlayerModel playerModel;
    [SerializeField] private CoinController coinController;
    [SerializeField] private PauseController pauseController;

    [Inject]
    private ISaveController _saveController;
    private readonly CompositeDisposable _disposables = new();
    public static ReactiveProperty<bool> IsGamePaused = new(false);
    private InputSystem_Actions _actions;

    public static bool loadGame = false;

    private void Awake()
    {
        if (playerController == null || playerModel == null || coinController == null)
            return;
        InitializeInput();
        InitializeModels();
        SubscribeToEvents();
    }

    /// <summary>
    /// Инициализация системы ввода.
    /// </summary>
    private void InitializeInput()
    {
        if(_actions == null)
            _actions = new InputSystem_Actions();
        _actions.Player.SetCallbacks(playerController);
        _actions.UI.SetCallbacks(this);
        _actions.Enable();
    }

    /// <summary>
    /// Инициализация моделей игрока.
    /// </summary>
    private void InitializeModels()
    {
        playerModel.Initialize(playerController.OnMoveStream, playerController.OnJumpStream);
        playerController.Initialize();
    }

    /// <summary>
    /// Подписка на игровые события.
    /// </summary>
    private void SubscribeToEvents()
    {
        playerController.OnCoinCollect
            .Subscribe(async _ =>
            {
                coinController.AddCoin();
                await _saveController.SaveAsync(playerController);
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
                   await _saveController.SaveAsync(playerController);
            })
            .AddTo(_disposables);
    }

    private async void Start()
    {
        if (loadGame){
            await _saveController.LoadAsync();
            _saveController.LoadSave(playerController, coinController); 
        }
    }
    private void OnDestroy()
    {
        _disposables.Dispose();
        _actions.Disable();
        IsGamePaused.Value = false;
        loadGame = false;
    }

    /// <summary>
    /// Обрабатывает нажатие кнопки паузы.
    /// </summary>
    /// <param name="context">Контекст действия ввода.</param>
    public void OnPause(InputAction.CallbackContext context)
    {
        if(context.performed)
            HandlePause();
    }
    
    /// <summary>
    /// Переключает или устанавливает состояние паузы.
    /// </summary>
    /// <param name="value">Новое значение паузы (опционально).</param>
    public void HandlePause(bool? value = null){
        IsGamePaused.Value = value!=null ? (bool)value:!IsGamePaused.Value;
    }
} 