using UnityEngine;
using Pathfinding;

public class TitleScreenPlayer : MonoBehaviour
{
    Animator animator;
    SpriteRenderer spriteRenderer;
    Rigidbody2D rigidbody2d;

    private AIPath path;
    private GameObject[] npcObjects;
    private GameObject closestNPC = null;
    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidbody2d = GetComponent<Rigidbody2D>();
        npcObjects = GameObject.FindGameObjectsWithTag("NPC");
        path = GetComponent<AIPath>();
    }

    void Update()
    {
        findClosestNPC();
        path.maxSpeed = 2f;
        path.destination = closestNPC.transform.position;
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
}
