using UnityEngine;

public class CandleTrigger : MonoBehaviour
{
    CandleController controller;

    private void Awake()
    {
        controller = GetComponent<CandleController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        controller.enabled = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        controller.enabled = false;
    }

}
