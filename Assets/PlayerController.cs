using R3;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerModel))]
public class PlayerController : MonoBehaviour,InputSystem_Actions.IPlayerActions
{
    private readonly CompositeDisposable _disposables = new();
    private readonly ReactiveProperty<Vector2> _move = new();
    private readonly Subject<Unit> _jump= new();
    private readonly Subject<Collider2D> _onTriggerEnter2D = new();

    public Observable<Vector2> OnMoveStream=>_move;
    public Observable<Unit> OnJumpStream =>_jump;
    public Observable<Unit> OnCoinCollect { get; private set; }

    private InputSystem_Actions _actions;
    private PlayerModel _playerModel;

    private void Awake()
    {
        _playerModel = GetComponent<PlayerModel>();

        if(_actions == null)
            _actions = new InputSystem_Actions();

        _actions.Player.SetCallbacks(this);
        _actions.Enable();

        _playerModel.Initialize(OnMoveStream, OnJumpStream);
        
        OnJumpStream.ThrottleFirst(System.TimeSpan.FromSeconds(0.25))
        .Subscribe(move => Debug.Log("Jumped"))
        .AddTo(_disposables);

        OnCoinCollect = _onTriggerEnter2D
            .Select(collision => collision.gameObject.GetComponent<ICollectable>())
            .Where(collectable => collectable != null)
            .Do(collectable => collectable.Collect())
            .Select(_ => Unit.Default)
            .Share();

        OnCoinCollect
            .Subscribe(_ => Debug.Log("!"))
            .AddTo(_disposables);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        _onTriggerEnter2D.OnNext(collision);
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _jump.OnNext(Unit.Default);
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _move.Value = context.ReadValue<Vector2>();
    }

    private void OnDestroy()
    {
        _disposables.Dispose();
        _move.Dispose();
        _jump.Dispose();
        _onTriggerEnter2D.Dispose();
        _actions.Disable();
    }
} 