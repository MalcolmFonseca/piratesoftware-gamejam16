using UnityEngine;
using Pathfinding;
using System.Collections;
using System.IO;

public class FlightNPC : MonoBehaviour
{
    private float sanity = 1;

    Animator animator;
    Rigidbody2D rigidbody2d;

    //Pathfinding
    private bool inChase = false;
    private bool pathFinished = false;
    private bool idle = true;
    private Vector3 randomPoint;
    private bool onPath = false;
    private AIPath path;
    private float maxMoveSpeed = 2;
    private float currentSpeed;
    [SerializeField] private Transform[] targets;
    [SerializeField] private float[] timings;

    private void Start()
    {
        animator = GetComponent<Animator>();
        rigidbody2d = GetComponent<Rigidbody2D>();
        path = GetComponent<AIPath>();
    }

    private void Update()
    {
        path.maxSpeed = maxMoveSpeed;
        if (inChase)
        {
            //try to find nearest person
            animator.SetBool("Running", true);
        } else if (onPath)
        {
            animator.SetBool("Running", false);
            //go to desired target
            path.destination = targets[0].position;
        } else
        {
            //wander in room
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

        //Animation
        if (path.velocity.magnitude > 0) {
            animator.SetBool("Moving", true);
        } else
        {
            animator.SetBool("Moving", false);
        }

        animator.SetFloat("Sanity", sanity);
    }
    
    private void FixedUpdate()
    {
        
    }

    IEnumerator setIdle()
    {
        yield return new WaitForSeconds(5f);
        idle = true;
        pathFinished = false;
    }
}
