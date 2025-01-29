using System.Collections;
using UnityEngine;

public class FlashLight : MonoBehaviour
{
    [SerializeField]
    GameObject parent;

    GuardMove guard;
    LayerMask playerLayer;
    float timePassed = 0f;
    float hitInterval = 1f;

    private void Awake()
    {
        guard = parent.GetComponent<GuardMove>();
        playerLayer = LayerMask.GetMask("Player");
    }


    void Update()
    {
        // Flashlight angle
        Vector2 velocity = guard.velocity;
        float angle = Mathf.Atan2 (-1 * velocity.x, velocity.y) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // Track time to only call damage once every second
        timePassed += Time.deltaTime;


        // Call player damage event on raycast hit
        RaycastHit2D hit = Physics2D.Raycast(transform.position, velocity, 4f, playerLayer);
        if(hit && timePassed >= hitInterval)
        {
            timePassed = 0f;
            GameEvents.instance.TakeDamage();
        }

    }

}
