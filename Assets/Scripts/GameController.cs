using R3;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class GameController : MonoBehaviour,InputSystem_Actions.IUIActions
{

    [SerializeField] private PlayerController _playerController;

    [SerializeField] private PlayerModel _playerModel;
    [SerializeField] private CoinController _coinController;
    [SerializeField] private PauseController _pauseController;

    [Inject]
    private ISaveController _saveController;
    private readonly CompositeDisposable _disposables = new();
    public static ReactiveProperty<bool> IsGamePaused = new(false);
    private InputSystem_Actions _actions;

    public static bool IsGameLoaded = false;

    private void Awake()
    {
        if (_playerController == null || _playerModel == null || _coinController == null)
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
        _actions.Player.SetCallbacks(_playerController);
        _actions.UI.SetCallbacks(this);
        _actions.Enable();
    }

    /// <summary>
    /// Инициализация моделей игрока.
    /// </summary>
    private void InitializeModels()
    {
        _playerModel.Initialize(_playerController.OnMoveStream, _playerController.OnJumpStream);
        _playerController.Initialize();
    }

    /// <summary>
    /// Подписка на игровые события.
    /// </summary>
    private void SubscribeToEvents()
    {
        _playerController.OnCoinCollect
            .Subscribe(async _ =>
            {
                _coinController.AddCoin();
                await _saveController.SaveAsync(_playerController,_playerModel);
            })
            .AddTo(_disposables);

        _playerController.OnJumpStream
            .ThrottleFirst(TimeSpan.FromSeconds(0.25))
            .Subscribe(_ => {})
            .AddTo(_disposables);

        IsGamePaused
            .Subscribe(async paused => {
               _playerModel.HandlePause(paused);
               _pauseController.SetPauseScreenVisible(paused);
               if (paused)
                   await _saveController.SaveAsync(_playerController, _playerModel);
            })
            .AddTo(_disposables);
    }

    private async void Start()
    {
        if (IsGameLoaded){
            await _saveController.LoadAsync();
            _saveController.LoadSave(_playerController,  _playerModel, _coinController); 
        }
    }
    private void OnDestroy()
    {
        _disposables.Dispose();
        _actions.Disable();
        IsGamePaused.Value = false;
        IsGameLoaded = false;
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