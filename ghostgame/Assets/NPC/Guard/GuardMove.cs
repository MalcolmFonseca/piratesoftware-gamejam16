using UnityEngine;
using Pathfinding;
using System;
using Pathfinding;
using UnityEngine.InputSystem.Processors;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;

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

    float sanity;
    Slider slider;
    Light2D light;
    FlashLight flashLightDamage;

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
        sanity = 100;
        slider = gameObject.GetComponentInChildren<Slider>();
        light = gameObject.GetComponentInChildren<Light2D>();
        flashLightDamage = gameObject.GetComponentInChildren<FlashLight>();
    }


    private void Update()
    {

        if (currentState != StateMachine.Scared && currentState != StateMachine.Dead)
        {
            Vector2 direction = ghost.transform.position - transform.position;

            // Call player damage event on raycast hit
            RaycastHit2D playerHit = Physics2D.Raycast(transform.position, direction, 5f, wallLayer);
            if (playerHit && playerHit.collider.tag == "Player")
            {
                currentState = StateMachine.Chase;
            }
            else
            {
                currentState = StateMachine.Patrol;
            }
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
            case StateMachine.Dead:
                Dead();
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
        Patrolling();
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
        Patrolling();
    }

    void Dead()
    {
        aiLerp.canMove = false;
    }

    void Patrolling()
    {
        if (aiLerp.destination != targetOne.position && aiLerp.destination != targetTwo.position)
        {
            SetGuardDestination(mostRecentDestination);
        }

        if (aiLerp.destination == targetOne.position && aiLerp.reachedDestination)
        {
            SetGuardDestination(targetTwo);
            mostRecentDestination = targetTwo;
        }
        else if (aiLerp.reachedDestination)
        {
            SetGuardDestination(targetOne);
            mostRecentDestination = targetOne;
        }
    }

    private void SetGuardDestination(Transform target)
    {
        aiLerp.destination = target.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "SanityHit")
        {
            // adjust sanity
            sanity -= 25;
            slider.value = sanity;

            // Check if there needs to be a state change
            if (sanity <= 0)
            {
                anim.SetBool("isScared", false);
                anim.SetBool("isDead", true);
                currentState = StateMachine.Dead;
            }
            else if (sanity < 26)
            {
                anim.SetBool("IsAngry", false);
                anim.SetBool("isScared", true);
                currentState = StateMachine.Scared;
                light.enabled = false;
                flashLightDamage.enabled = false;
            }

            // Test hit
            Debug.Log("Hit");
        }

    }

}
