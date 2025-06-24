using R3;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerModel))]
public class PlayerController : MonoBehaviour, InputSystem_Actions.IPlayerActions
{
    private readonly CompositeDisposable _disposables = new();
    private readonly Subject<Vector2> _move = new();
    private readonly Subject<Unit> _jump = new();

    public Observable<Vector2> OnMoveStream => _move;
    public Observable<Unit> OnJumpStream => _jump;

    private InputSystem_Actions _actions;
    private PlayerModel _playerModel;

    private void Awake()
    {
        _playerModel = GetComponent<PlayerModel>();

        if (_actions == null)
            _actions = new InputSystem_Actions();

        _actions.Player.SetCallbacks(this);
        _actions.Enable();

        _playerModel.Initialize(OnMoveStream, OnJumpStream);

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
        _move.OnNext(context.ReadValue<Vector2>());
    }

    private void OnDestroy()
    {
        _disposables.Dispose();
        _move.Dispose();
        _jump.Dispose();
        _actions.Disable();
        _actions.Dispose();
    }
}