using UnityEngine;
using Pathfinding;

public class FlightNPC : MonoBehaviour
{
    private float sanity = 1;

    Animator animator;
    Rigidbody2D rigidbody2d;

    //Pathfinding
    private AIPath path;
    private float maxMoveSpeed = 2;
    private float currentSpeed;
    public Transform[] targets;
    public float[] timings;

    private void Start()
    {
        animator = GetComponent<Animator>();
        rigidbody2d = GetComponent<Rigidbody2D>();
        path = GetComponent<AIPath>();
    }

    private void Update()
    {
        path.maxSpeed = maxMoveSpeed;
        path.destination = targets[0].position;
        //Animation
        if (path.velocity.magnitude > 0) {
            animator.SetBool("Moving", true);
        } else
        {
            animator.SetBool("Moving", false);
        }
    }
    
    private void FixedUpdate()
    {
        
    }
}
