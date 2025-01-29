using UnityEngine;
using Pathfinding;
using System;
using Pathfinding;
using UnityEngine.InputSystem.Processors;
using System.Collections;

public class GuardMove : MonoBehaviour
{
    public Vector2 velocity;
    private AILerp aiLerp;
    [SerializeField]
    Transform targetOne;
    [SerializeField]
    Transform targetTwo;
    [SerializeField]
    Transform ghost;
    Transform mostRecentDestination;
    Animator anim;
    SpriteRenderer spriteRenderer;
    public bool isAngry;
    LayerMask wallLayer;

    public enum StateMachine
    {
        Patrol,
        Chase,
        Scared,
        Dead
    }

    public StateMachine currentState;

    private void Awake()
    {
        aiLerp = GetComponent<AILerp>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        wallLayer = LayerMask.GetMask("Obstacle");
    }

    private void Start()
    {
        currentState = StateMachine.Patrol;
        SetGuardDestination(targetOne);
        mostRecentDestination = targetOne;
    }


    private void Update()
    {

        Vector2 direction = ghost.transform.position - transform.position;

        // Call player damage event on raycast hit
        RaycastHit2D playerHit = Physics2D.Raycast(transform.position, direction, 5f, wallLayer);
        if (playerHit && playerHit.collider.tag == "Player")
        {
            currentState = StateMachine.Chase;
        } else
        {
            currentState = StateMachine.Patrol;
        }


        switch (currentState)
        {
            case StateMachine.Patrol:
                Patrol();
                break;
            case StateMachine.Chase:
                Chase();
                break;
            case StateMachine.Scared:
                Scared();
                break;
        }

        velocity = aiLerp.velocity;

        anim.SetFloat("speed", aiLerp.velocity.magnitude);
        if (aiLerp.velocity.x < 0f)
        {
            spriteRenderer.flipX = false;
        } else
        {
            spriteRenderer.flipX = true;
        }
    }

    void Patrol()
    {
        // Included for initial start, and to ensure once no longer chasing the ghost and comes back to patrol
        // the destination resets off of the player's last seen position
        if(aiLerp.destination != targetOne.position && aiLerp.destination != targetTwo.position)
        {
            SetGuardDestination(mostRecentDestination);
        }

        if(aiLerp.destination == targetOne.position && aiLerp.reachedDestination)
        {
            SetGuardDestination(targetTwo);
            mostRecentDestination = targetTwo;
        }
        else if(aiLerp.reachedDestination)
        {
            SetGuardDestination(targetOne);
            mostRecentDestination = targetOne;
        }
        anim.SetBool("isAngry", false);
        isAngry = false;
    }

    void Chase()
    {
        isAngry = true;
        SetGuardDestination(ghost);
        anim.SetBool("isAngry", true);
    }

    void Scared()
    {
        isAngry = false;
        aiLerp.canMove = false;
        anim.SetBool("IsAngry", false);
        anim.SetBool("isScared", true);
    }

    private void SetGuardDestination(Transform target)
    {
        aiLerp.destination = target.position;
    }

}
