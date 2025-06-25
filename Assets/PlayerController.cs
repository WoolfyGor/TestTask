using R3;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour,InputSystem_Actions.IPlayerActions
{
    private readonly ReactiveProperty<Vector2> _move = new();
    private readonly ReactiveProperty<bool> _grounded = new();
    private readonly Subject<Unit> _jump= new();
    private readonly Subject<Collider2D> _onTriggerEnter2D = new();

    [SerializeField] private float _groundCheckDistance = 0.2f;
    [SerializeField] private LayerMask _groundLayer;

    public Observable<Vector2> OnMoveStream=>_move;
    public Observable<Unit> OnJumpStream =>_jump;
    public Observable<Unit> OnCoinCollect { get; private set; }


    public void Initialize(){

        OnCoinCollect = _onTriggerEnter2D
            .Select(collision => collision.gameObject.GetComponent<ICollectable>())
            .Where(collectable => collectable != null)
            .Do(collectable => collectable.Collect())
            .Select(_ => Unit.Default)
            .Share();
        }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        _onTriggerEnter2D.OnNext(collision);
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Vector2 origin = transform.position;
            RaycastHit2D hit = Physics2D.Raycast(origin, -transform.up, _groundCheckDistance, _groundLayer);
            _grounded.Value = hit.collider != null;
            if (_grounded.Value)
            {
                _jump.OnNext(Unit.Default);
            }
        }
    }


    public void OnMove(InputAction.CallbackContext context)
    {
        _move.Value = context.ReadValue<Vector2>();
    }

    private void OnDestroy()
    {
        _move.Dispose();
        _jump.Dispose();
        _onTriggerEnter2D.Dispose();
    }
    void OnDrawGizmosSelected(){
        Vector2 origin = transform.position;
        Vector2 endPoint = origin - new Vector2(0,_groundCheckDistance);
        RaycastHit2D hit = Physics2D.Raycast(origin, -transform.up, _groundCheckDistance, _groundLayer);

        Gizmos.color = hit.collider != null?Color.green:Color.red;
        Gizmos.DrawLine(origin, endPoint);
    }
} 