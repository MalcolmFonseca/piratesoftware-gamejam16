using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    // Interact Event parameter set by trigger
    GameObject interactableObject;

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
    bool isInvisible;
    bool inWall;

    void Awake()
    {
        // Components and Movement
        anim = GetComponent<Animator>();
        inputControls = new InputSystem_Actions();
        rigidBody2d = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        // Map boundaries set by two empty objects
        xMax = maxBoundaryObj.transform.position.x;
        yMax = maxBoundaryObj.transform.position.y;
        xMin = minBoundaryObj.transform.position.x;
        yMin = minBoundaryObj.transform.position.y;
        // invisibility
        isInvisible = false;
        inWall = false;
    }

    private void OnEnable()
    {
        // Movement
        moveAction = inputControls.Player.Move;
        moveAction.Enable();
        // Interact 'E'
        interact = inputControls.Player.Interact;
        interact.Enable();
        interact.performed += Interact;
        // Invisibility 'Q'
        invisibility = inputControls.Player.Ability;
        invisibility.Enable();
        invisibility.performed += Invisibility;
    }

    private void OnDisable()
    {
        // Interact 'E'
        interact.performed -= Interact;
        interact.Disable();
        // Invisibility 'Q'
        invisibility.performed -= Invisibility;
        invisibility.Disable();
        // Movement
        moveAction.Disable();
    }

    void Update()
    {
        // Movement
        move = moveAction.ReadValue<Vector2>();
        if(!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            moveDirection.Set(move.x, move.y);
            moveDirection.Normalize();
        }

        // Animation Direction
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
        // Boundary restrictions based on empty object placements
        Vector2 position = new Vector2(
            Mathf.Clamp(rigidBody2d.position.x + move.x * moveSpeed * Time.deltaTime, xMin, xMax),
            Mathf.Clamp(rigidBody2d.position.y + move.y * moveSpeed * Time.deltaTime, yMin, yMax)
            );
        rigidBody2d.MovePosition(position);
    }

    // Interact Event 'E'
    private void Interact(InputAction.CallbackContext context)
    {
        anim.SetTrigger("interact");

        // Call interact event (in GameEvents)
        if(interactableObject != null)
        {
            GameEvents.instance.Interact(interactableObject);
        }
    }




    /*
        Invisibility requires ground collisions everywhere between the map rooms.
        The ground collisions cannot use the composite collider for merged tiles
            - the ghost has a trigger slightly larger than 1 tile and always needs
              to witness a collision event when in the walls
        Restricting the ghosts invisibility toggle ensures they don't get stuck in the wall
     */


    // Invisibility set to 'Q'
    private void Invisibility(InputAction.CallbackContext context) 
    {

        if (isInvisible && !inWall)
        {
            isInvisible = false;
            TransparencyChange(1f);
            ManageColliders(isInvisible);
            spriteRenderer.sortingLayerName = "Collisions";
        } else
        {
            isInvisible = true;
            TransparencyChange(0.27f);
            ManageColliders(isInvisible);
            spriteRenderer.sortingLayerName = "Invisible";
        }
    }

    private void TransparencyChange(float transparency)
    {
        Color color = spriteRenderer.color;
        color.a = transparency;
        spriteRenderer.color = color;
    }

    private void ManageColliders(bool isInvisible)
    {
        triggerCollider.enabled = isInvisible;
        boxCollider2d.enabled = !isInvisible;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag != "Interact")
        {
            inWall = true;
        } else
        {
            interactableObject = collision.gameObject;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag != "Interact")
        {
            inWall = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag != "Interact")
        {
            inWall = false;
        } else
        {
            interactableObject = null;
        }
    }
}
