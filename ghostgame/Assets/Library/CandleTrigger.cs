using UnityEngine;

public class CandleTrigger : MonoBehaviour
{
    CandleController controller;

    private void Awake()
    {
        controller = GetComponent<CandleController>();
        controller.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            controller.enabled = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            controller.enabled = false;
        }
    }

}
