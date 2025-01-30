using UnityEngine;
using Pathfinding;
using System;
using Pathfinding;
using UnityEngine.InputSystem.Processors;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;
using Unity.VisualScripting;

public class GuardMove : MonoBehaviour
{
    float timePassed;
    bool inDarkness;
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
    BoxCollider2D collider;

    float sanity;
    float maxSanity;
    Slider slider;
    Light2D light;
    FlashLight flashLightDamage;

    Coroutine runningCoroutine;

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
        timePassed = 0;
        inDarkness = true;
    }

    private void Start()
    {
        // State and destination
        currentState = StateMachine.Patrol;
        SetGuardDestination(targetOne);
        mostRecentDestination = targetOne;
        
        // Sanity/flashlight
        sanity = 100;
        maxSanity = 100;
        slider = gameObject.GetComponentInChildren<Slider>();
        light = gameObject.GetComponentInChildren<Light2D>();
        flashLightDamage = gameObject.GetComponentInChildren<FlashLight>();
        collider = GetComponent<BoxCollider2D>();
    }


    private void Update()
    {
        if(0 < sanity && sanity <= 25)
        {
            currentState = StateMachine.Scared;
        } else if(sanity <= 0)
        {
            currentState = StateMachine.Dead;
        }

        timePassed += Time.deltaTime;

        if (timePassed > 0.5f)
        {
            timePassed = 0;
            if (inDarkness) { 
            
                ChangeSanity(-1f);
            } else
            {
                ChangeSanity(1f);
            }
            // Checking if in patrol/angry state, if can see ghost, then chase
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

        // Used for flashligh direction
        velocity = aiLerp.velocity;


        // Animation/sprite changing
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
        Patrolling();
        isAngry = false;
        anim.SetBool("isAngry", false);
        anim.SetBool("isScared", true);
        light.enabled = false;
        flashLightDamage.enabled = false;
    }

    void Dead()
    {
        anim.SetBool("isScared", false);
        anim.SetBool("isDead", true);
        aiLerp.canMove = false;
        collider.enabled = false;
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
        // Interact object thrown
        if (collision.gameObject.tag == "SanityHit")
        {
            ChangeSanity(-10);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Light")
        {
            inDarkness = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Light")
        {
            inDarkness = true;
        }
        
    }

    void ChangeSanity(float change)
    {

        sanity += change;
        if (sanity > maxSanity)
        {
            sanity = maxSanity;
        }
        slider.value = sanity;
        Debug.Log("Sanity: " + sanity);
        
    }

    IEnumerator LightingSanityChange(float change)
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            ChangeSanity(change);
        }
    }

}
