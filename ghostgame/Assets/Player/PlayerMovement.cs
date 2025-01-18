using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public InputSystem_Actions inputControls;
    InputAction moveAction;
    Vector2 move;
    Vector2 moveDirection = new Vector2 (0, 0);
    [SerializeField]
    float moveSpeed;
    Animator anim;
    Rigidbody2D rigidBody2d;
    SpriteRenderer spriteRenderer;

    void Awake()
    {
        anim = GetComponent<Animator>();
        inputControls = new InputSystem_Actions();
        rigidBody2d = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        moveAction = inputControls.Player.Move;
        moveAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
    }

    void Update()
    {
        move = moveAction.ReadValue<Vector2>();

        if(!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            moveDirection.Set(move.x, move.y);
            moveDirection.Normalize();
        }

        if (moveDirection.x > 0.0f) 
        {
            spriteRenderer.flipX = true;
        } else if (moveDirection.x < 0.0f)
        {
            spriteRenderer.flipX = false;
        }

        anim.SetFloat("speed", move.magnitude);
    }

    private void FixedUpdate()
    {
        Vector2 position = (Vector2)rigidBody2d.position + move * moveSpeed * Time.deltaTime;
        rigidBody2d.MovePosition(position);
    }
}
