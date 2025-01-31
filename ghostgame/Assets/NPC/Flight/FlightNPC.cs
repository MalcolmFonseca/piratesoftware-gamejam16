using UnityEngine;
using Pathfinding;
using System.Collections;
using System.IO;
using Unity.VisualScripting;
using UnityEngine.UI;

public class FlightNPC : MonoBehaviour
{
    private float sanity = 1f;
    private bool inDarkness = true;

    Animator animator;
    SpriteRenderer spriteRenderer;
    Rigidbody2D rigidbody2d;

    //Pathfinding
    private bool inChase = true;
    private bool pathFinished = false;
    private bool idle = true;
    private Vector3 wanderTarget;
    private bool onPath = false;
    private AIPath path;
    private float maxMoveSpeed = 1;
    [SerializeField] private Transform[] targets;
    private int targetIndex = 0;

    private GameObject[] npcObjects;
    private GameObject closestNPC = null;
    private GameObject playerObject;
    LayerMask playerLayer;
    LayerMask obstacleLayer;

    private AudioSource source;
    [SerializeField] private AudioClip clip;

    Slider slider;

    [SerializeField] GameObject deadPrefab;
    private void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidbody2d = GetComponent<Rigidbody2D>();
        path = GetComponent<AIPath>();
        npcObjects = GameObject.FindGameObjectsWithTag("NPC");
        playerObject = GameObject.FindGameObjectWithTag("Player");
        source = GetComponent<AudioSource>();
        playerLayer = LayerMask.GetMask("Player");
        obstacleLayer = LayerMask.GetMask("Obstacle");
        slider = gameObject.GetComponentInChildren<Slider>();
    }

    private void Update()
    {
        if (sanity == 0)
        {
            //die
            Instantiate(deadPrefab, transform.position, Quaternion.identity);
            GameEvents.instance.NpcDead(gameObject);
            Object.Destroy(this.gameObject);
        }

        //check if ghost in vision
        Vector3 playerPosition = playerObject.transform.position + new Vector3(0,.1f,0);
        RaycastHit2D playerHit = Physics2D.Raycast(transform.position, playerPosition - transform.position, 8f, playerLayer);
        RaycastHit2D obstacleHit = Physics2D.Raycast(transform.position, playerPosition - transform.position, 8f, obstacleLayer);
        if (playerHit && playerHit.collider.tag == "Player" && !playerHit.collider.isTrigger && !obstacleHit)
        {
            if (!inChase)
            {
                //play scream sound
                source.PlayOneShot(clip);
            }
            ChangeSanity(-.05f * Time.deltaTime);
            inChase = true;
        } else if (inChase)
        {
            inChase = false;
            idle = true;
        }

        findClosestNPC();
        //increase sanity if near friends
        if (calcDistance(closestNPC.transform.position)<5)
        {
            ChangeSanity(.01f * Time.deltaTime);
        }

        //change sanity with lighting
        if (inDarkness)
        {
            ChangeSanity(-.01f * Time.deltaTime);
        }
        else
        {
            ChangeSanity(.01f * Time.deltaTime);
        }

        //--------------------------AI Movement----------------------------
        path.maxSpeed = maxMoveSpeed+sanity;
        if (inChase)//---------------Chased------------------
        {
            //faster in chase
            path.maxSpeed = maxMoveSpeed + sanity + .5f;
            //try to find nearest person
            GameObject closestGuard = findClosestGuard();
            if (closestGuard != null)
            {
                path.destination = closestGuard.transform.position;
            } else
            {
                //all guards dead, just try to run away from ghost
                Vector3 playerVector = playerObject.transform.position - transform.position;
                playerVector.x = playerVector.x * -1;
                playerVector.y = playerVector.y * -1;
                path.destination = playerVector+transform.position;
            }
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
                    wanderTarget = closestNPC.transform.position;
                    wanderTarget.x += (Random.Range(0, 2) * 2 - 1) * 3;
                    wanderTarget.y += (Random.Range(0, 2) * 2 - 1) * 3;
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

    private void findClosestNPC()
    {
        float distance = 100000;
        foreach (GameObject npc in npcObjects)
        {
            float tempDistance = calcDistance(npc.transform.position);
            if (tempDistance < distance && tempDistance != 0)
            {
                distance = tempDistance;
                closestNPC = npc;
            }
        }
    }

    private GameObject findClosestGuard() 
    {
        float distance = 100000;
        GameObject closestGuard = null;
        foreach (GameObject npc in npcObjects)
        {
            GuardMove guard = npc.GetComponent<GuardMove>();
            if (guard != null) 
            {
                float tempDistance = calcDistance(npc.transform.position);
                if (tempDistance < distance && tempDistance != 0)
                {
                    distance = tempDistance;
                    closestGuard = npc;
                }
            }
        }

        return closestGuard;
    }

    private void ChangeSanity(float change)
    {
        if (sanity + change < 0 || sanity + change > 1)
        {
            return;
        }
        else
        {
            sanity += change;
        }
        slider.value = sanity;
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Light")
        {
            inDarkness = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Light")
        {
            inDarkness = true;
        }
    }
}
