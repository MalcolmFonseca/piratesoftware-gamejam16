using UnityEngine;
using Pathfinding;
using System.Collections;

public class TitleScreenPlayer : MonoBehaviour
{
    Animator animator;
    SpriteRenderer spriteRenderer;
    Rigidbody2D rigidbody2d;

    private AIPath path;
    private GameObject[] npcObjects;
    private GameObject closestNPC = null;
    private bool left = true;
    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidbody2d = GetComponent<Rigidbody2D>();
        npcObjects = GameObject.FindGameObjectsWithTag("NPC");
        path = GetComponent<AIPath>();
        StartCoroutine(changeSide());
    }

    void Update()
    {
        findClosestNPC();
        path.maxSpeed = 2f;
        path.destination = closestNPC.transform.position;

        if (path.targetDirection.x < 0)
        /*its obsolete but its the only attribute of aipath that works for this so hopefully this doesnt break
        desired velocity cant return negative and steeringtarget doesnt update immediately*/
        {
            spriteRenderer.flipX = false;
        }
        else
        {
            spriteRenderer.flipX = true;
        }
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

    IEnumerator changeSide()
    {
        yield return new WaitForSeconds(4f);
        if (left)
        {
            transform.position = new Vector2(9, 0);
        } else
        {
            transform.position = new Vector2(-9, 0);
        }
        left = !left;
        StartCoroutine(changeSide());
    }
}
