using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public sealed class PlayerController2D : MonoBehaviour
{
    [SerializeField, Min(0f)] private float moveSpeed = 5f;
    [SerializeField] private Transform facingIndicator;

    private Rigidbody2D body;
    private Vector2 movementInput;
    private Vector2 facingDirection = Vector2.down;
    private bool gameplayEnabled = true;

    public Vector2 FacingDirection => facingDirection;

    public void SetGameplayEnabled(bool enabled)
    {
        gameplayEnabled = enabled;
        if (!enabled)
        {
            movementInput = Vector2.zero;
        }
    }
    public Vector2 MovementInput => movementInput;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        body.gravityScale = 0f;
        body.freezeRotation = true;
        body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    private void Update()
    {
        if (!gameplayEnabled)
        {
            movementInput = Vector2.zero;
            return;
        }

        movementInput = NormalizeInput(new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")));

        if (movementInput.sqrMagnitude > 0.0001f)
        {
            facingDirection = SelectFacingDirection(movementInput);
            UpdateFacingIndicator();
        }
    }

    private void FixedUpdate()
    {
        if (!gameplayEnabled)
        {
            return;
        }

        body.MovePosition(body.position + movementInput * moveSpeed * Time.fixedDeltaTime);
    }

    public static Vector2 NormalizeInput(Vector2 input)
    {
        return input.sqrMagnitude > 1f ? input.normalized : input;
    }

    private static Vector2 SelectFacingDirection(Vector2 input)
    {
        if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
        {
            return new Vector2(Mathf.Sign(input.x), 0f);
        }

        return new Vector2(0f, Mathf.Sign(input.y));
    }

    private void UpdateFacingIndicator()
    {
        if (facingIndicator == null)
        {
            return;
        }

        facingIndicator.localPosition = facingDirection * 0.62f;
        facingIndicator.localRotation = Quaternion.FromToRotation(Vector2.up, facingDirection);
    }
}
