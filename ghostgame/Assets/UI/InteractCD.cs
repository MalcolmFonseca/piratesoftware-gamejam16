using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class InteractCD : MonoBehaviour
{

    [SerializeField]
    private Image imageCD;
    [SerializeField]
    private TMP_Text textCD;

    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        textCD.gameObject.SetActive(false);
        imageCD.fillAmount = 0;
        GameEvents.instance.onInteract += StartCooldown;
    }

    private void OnDestroy()
    {
        GameEvents.instance.onInteract -= StartCooldown;
    }

    void StartCooldown(GameObject gameObject)
    {
        StartCoroutine(Cooldown());
    }

    IEnumerator Cooldown()
    {
        textCD.gameObject.SetActive(true);
        imageCD.fillAmount += 1;

        float cooldownTime = 5f;
        float timePassed = 0;

        while (timePassed < cooldownTime)
        {
            textCD.text = Mathf.RoundToInt(cooldownTime - timePassed).ToString();
            imageCD.fillAmount = (cooldownTime - timePassed)/cooldownTime;
            timePassed += Time.deltaTime;
            yield return null;
        }

        imageCD.fillAmount = 0;
        textCD.gameObject.SetActive(false);

    }



}
