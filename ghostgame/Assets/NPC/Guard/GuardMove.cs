using UnityEngine;
using Pathfinding;
using System;
using Pathfinding;
using UnityEngine.InputSystem.Processors;

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
    Animator anim;
    SpriteRenderer spriteRenderer;

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
    }

    private void Start()
    {
        currentState = StateMachine.Patrol;
    }

    private void Update()
    {
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
            SetGuardDestination(targetOne);
        }

        if(aiLerp.destination == targetOne.position && aiLerp.reachedDestination)
        {
            SetGuardDestination(targetTwo);
        }
        else if(aiLerp.reachedDestination)
        {
            SetGuardDestination(targetOne);
        }
        anim.SetBool("isAngry", false);
    }

    void Chase()
    {
        SetGuardDestination(ghost);
        anim.SetBool("isAngry", true);
    }

    void Scared()
    {
        aiLerp.canMove = false;
        anim.SetBool("IsAngry", false);
        anim.SetBool("isScared", true);
    }

    private void SetGuardDestination(Transform target)
    {
        aiLerp.destination = target.position;
    }

}
