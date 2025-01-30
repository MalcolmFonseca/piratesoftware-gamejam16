using UnityEngine;
using UnityEngine.UI;

public class GuardSanity : MonoBehaviour
{

    float sanity;
    Slider slider;

    private void Start()
    {
        sanity = 100;
        slider = gameObject.GetComponentInChildren<Slider>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "SanityHit")
        {
            // adjust sanity
            sanity -= 25;
            slider.value = sanity;            

            // Test hit
            Debug.Log("Hit");
        }

    }

}
