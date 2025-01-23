using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    // Interact Event
    public static event Action<PlayerMovement> OnInteract;

    // Movement
    public InputSystem_Actions inputControls;
    InputAction moveAction;
    Vector2 move;
    Vector2 moveDirection = new Vector2 (0, 0);
    [SerializeField]
    float moveSpeed;
    Rigidbody2D rigidBody2d;
    Animator anim;
    // Ghost boundaries
    [SerializeField]
    GameObject maxBoundaryObj;
    float xMax, yMax;
    [SerializeField]
    GameObject minBoundaryObj;
    float xMin, yMin;
    // Other Inputs
    InputAction interact;
    InputAction invisibility;
    // Invisibility
    [SerializeField]
    BoxCollider2D boxCollider2d;
    [SerializeField]
    BoxCollider2D triggerCollider;
    SpriteRenderer spriteRenderer;
    Color color;
    bool isInvisible;
    bool inWall;

    void Awake()
    {
        anim = GetComponent<Animator>();
        inputControls = new InputSystem_Actions();
        rigidBody2d = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        // Map boundaries set by two empty objects
        xMax = maxBoundaryObj.transform.position.x;
        yMax = maxBoundaryObj.transform.position.y;
        xMin = minBoundaryObj.transform.position.x;
        yMin = minBoundaryObj.transform.position.y;
        isInvisible = false;
        inWall = false;
        color = spriteRenderer.color;
    }

    private void OnEnable()
    {
        moveAction = inputControls.Player.Move;
        moveAction.Enable();
        interact = inputControls.Player.Interact;
        interact.Enable();
        interact.performed += Interact;
        invisibility = inputControls.Player.Ability;
        invisibility.Enable();
        invisibility.performed += Invisibility;
    }

    private void OnDisable()
    {
        interact.performed -= Interact;
        interact.Disable();
        invisibility.performed -= Invisibility;
        invisibility.Disable();
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
        // Movement with boundary restrictions based on empty object placements
        Vector2 position = new Vector2(Mathf.Clamp(rigidBody2d.position.x + move.x * moveSpeed * Time.deltaTime, xMin, xMax),
            Mathf.Clamp(rigidBody2d.position.y + move.y * moveSpeed * Time.deltaTime, yMin, yMax));
        rigidBody2d.MovePosition(position);
    }

    // Interact set to 'E'
    private void Interact(InputAction.CallbackContext context)
    {
        anim.SetTrigger("interact");

        // Interactable environment objects subscribe to this event
        OnInteract?.Invoke(this);
    }

    /*
        Invisibility requires ground collisions everywhere between the map rooms.
        The ground collisions cannot use the composite collider for merged tiles
            - the ghost has a trigger slightly larger than 1 tile and always needs
              to witness a collision event when in the walls
        Restricting the ghosts invisibility toggle ensures they don't get stuck in the wall
     */
    // Ability set to 'Q'
    private void Invisibility(InputAction.CallbackContext context) 
    {

        if (isInvisible && !inWall)
        {
            isInvisible = false;
            color.a = 1;
            spriteRenderer.color = color;
            boxCollider2d.enabled = true;       // turn on collisions
            triggerCollider.enabled = false;    // turn off trigger
        } else
        {
            isInvisible = true;
            color.a = 0.27f;
            spriteRenderer.color = color;
            boxCollider2d.enabled = false;      // turn off collisions
            triggerCollider.enabled = true;     // turn on trigger
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag != "Candle")
        {
            inWall = true;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag != "Candle")
        {
            inWall = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag != "Candle")
        {
            inWall = false;
        }

    }

}
