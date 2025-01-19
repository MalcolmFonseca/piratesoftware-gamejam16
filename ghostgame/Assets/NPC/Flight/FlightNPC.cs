using UnityEngine;

public class FlightNPC : MonoBehaviour
{
    private float sanity = 1;

    Animator animator;
    Rigidbody2D rigidbody2d;

    private void Start()
    {
        animator = GetComponent<Animator>();
        rigidbody2d = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        
    }
    
    private void FixedUpdate()
    {
        
    }
}
