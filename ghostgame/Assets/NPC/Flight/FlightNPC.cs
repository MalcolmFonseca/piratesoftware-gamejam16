using UnityEngine;
using Pathfinding;
using System.Collections;
using System.IO;

public class FlightNPC : MonoBehaviour
{
    private float sanity = 1f;

    Animator animator;
    SpriteRenderer spriteRenderer;
    Rigidbody2D rigidbody2d;

    //Pathfinding
    private bool inChase = false;
    private bool pathFinished = false;
    private bool idle = true;
    private Vector3 wanderTarget;
    private bool onPath = false;
    private AIPath path;
    private float maxMoveSpeed = 2;
    [SerializeField] private Transform[] targets;
    private int targetIndex = 0;

    private GameObject[] npcObjects;

    private void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidbody2d = GetComponent<Rigidbody2D>();
        path = GetComponent<AIPath>();
        npcObjects = GameObject.FindGameObjectsWithTag("NPC");
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
                if (sanity >= .5)
                {
                    //pick random point to wander to
                    wanderTarget = Random.insideUnitSphere * 6;
                    wanderTarget.y = 0;
                    wanderTarget += transform.position;
                }
                else //if low on sanity seek closest friend
                {
                    float distance = 100000;
                    foreach (GameObject npc in npcObjects)
                    {
                        float tempDistance = calcDistance(npc.transform.position);
                        if (tempDistance<distance && tempDistance != 0) 
                        {
                            distance = tempDistance;
                            wanderTarget = npc.transform.position;
                            wanderTarget.x += (Random.Range(0, 2) * 2 - 1) * 3;
                            wanderTarget.y += (Random.Range(0, 2) * 2 - 1) * 3;
                        }
                    }
                }

                idle = false;
            } else
            {
                //roam to random point
                path.destination = wanderTarget;
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

    private float calcDistance(Vector3 point)
    {
        Vector3 distanceVector = point - transform.position;
        return distanceVector.magnitude;
    }
}
