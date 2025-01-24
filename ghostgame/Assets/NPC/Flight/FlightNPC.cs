using UnityEngine;
using Pathfinding;

public class FlightNPC : MonoBehaviour
{
    private float sanity = 1;

    Animator animator;
    Rigidbody2D rigidbody2d;

    //Pathfinding
    private AIPath path;
    private float moveSpeed = 2;
    public Transform target;

    private void Start()
    {
        animator = GetComponent<Animator>();
        rigidbody2d = GetComponent<Rigidbody2D>();
        path = GetComponent<AIPath>();  
    }

    private void Update()
    {
        path.maxSpeed = moveSpeed;
        path.destination = target.position;
    }
    
    private void FixedUpdate()
    {
        
    }
}
