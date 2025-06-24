using UnityEngine;
using R3;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerModel : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 5f;

    private Rigidbody2D rb;
    private readonly CompositeDisposable _disposables = new();

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Initialize(Observable<Vector2> moveStream, Observable<Unit> jumpStream)
    {
        Observable.EveryUpdate(UnityFrameProvider.FixedUpdate)
            .WithLatestFrom(moveStream, (_, direction) => direction)
            .Subscribe(Move)
            .AddTo(_disposables);

        jumpStream
            .Subscribe(_ => Jump())
            .AddTo(_disposables);
    }

    private void Move(Vector2 direction)
    {
        rb.linearVelocity = new Vector2(direction.x * moveSpeed, rb.linearVelocity.y);

        if (direction.x > 0.1f)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (direction.x < -0.1f)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    private void Jump()
    {
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    private void OnDestroy()
    {
        _disposables.Dispose();
    }
}