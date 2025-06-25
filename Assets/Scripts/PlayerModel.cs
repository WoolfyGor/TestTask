using UnityEngine;
using R3;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerModel : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 5f;

    private Rigidbody2D rb;
    private readonly CompositeDisposable _disposables = new();
    public bool paused = false;
    public Vector2 savedVelocity { get; private set; }
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

    }

    /// <summary>
    /// Инициализирует управление движением и прыжком игрока.
    /// </summary>
    /// <param name="moveStream">Поток событий движения.</param>
    /// <param name="jumpStream">Поток событий прыжка.</param>
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
        if (paused) return;

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

    /// <summary>
    /// Обрабатывает постановку и снятие паузы.
    /// </summary>
    /// <param name="state">Состояние паузы.</param>
    public void HandlePause(bool state){
        paused = state;
        if(rb == null)
            return;
        if(paused){
            savedVelocity = rb.linearVelocity;
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Static;

        }
        else{
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.linearVelocity = savedVelocity!=null?savedVelocity:Vector2.zero;
        }
    }

    private void Jump()
    {
        if (paused) return;
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }
  
    private void OnDestroy()
    {
        _disposables.Dispose();
    }
}