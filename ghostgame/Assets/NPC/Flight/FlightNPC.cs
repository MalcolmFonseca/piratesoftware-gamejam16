using UnityEngine;
using Pathfinding;
using System.Collections;
using System.IO;

public class FlightNPC : MonoBehaviour
{
    private float sanity = 1;

    Animator animator;
    SpriteRenderer spriteRenderer;
    Rigidbody2D rigidbody2d;

    //Pathfinding
    private bool inChase = false;
    private bool pathFinished = false;
    private bool idle = true;
    private Vector3 randomPoint;
    private bool onPath = false;
    private AIPath path;
    private float maxMoveSpeed = 2;
    [SerializeField] private Transform[] targets;
    private int targetIndex = 0;

    private void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidbody2d = GetComponent<Rigidbody2D>();
        path = GetComponent<AIPath>();
    }

    private void Update()
    {
        //--------------------------AI Movement----------------------------
        path.maxSpeed = maxMoveSpeed;
        if (inChase)//---------------Chased------------------
        {
            //try to find nearest person
            animator.SetBool("Running", true);
        } else if (onPath)//---------------Path to Room------------------
        {
            animator.SetBool("Running", false);
            //go to desired target
            path.destination = targets[targetIndex].position;
            //go back to roaming after reaching room
            if (path.velocity.magnitude == 0)
            {
                onPath = false;
            }
        }
        else//---------------Wandering in room------------------
        {
            animator.SetBool("Running", false);
            if (idle)
            {
                //pick random point to wander to
                randomPoint = Random.insideUnitSphere * 6;
                randomPoint.y = 0;
                randomPoint += transform.position;

                idle = false;
            } else
            {
                //roam to random point
                path.destination = randomPoint;
                if (path.velocity.magnitude  == 0 && !pathFinished)
                {
                    pathFinished = true;
                    StartCoroutine(setIdle());
                }
            }
 
        }

        //------------Animation-----------------
        if (path.velocity.magnitude > 0) {
            animator.SetBool("Moving", true);
        } else
        {
            animator.SetBool("Moving", false);
        }

        //direction
        if (path.targetDirection.x < 0) 
            /*its obsolete but its the only attribute of aipath that works for this so hopefully this doesnt break
            desired velocity cant return negative and steeringtarget doesnt update immediately*/
        {
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.flipX = false;
        }

        animator.SetFloat("Sanity", sanity);
    }

    IEnumerator setIdle()
    {
        yield return new WaitForSeconds(5f);
        idle = true;
        pathFinished = false;
    }

    //to be called by clock after reaching next hour
    public void changePath() 
    {
        //increment index but keep in bounds
        targetIndex += 1;
        if (targetIndex >= targets.Length) { targetIndex = 0; }
        onPath = true;
    }
}
