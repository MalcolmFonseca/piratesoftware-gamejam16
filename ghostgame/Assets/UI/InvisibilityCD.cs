using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class InvisibilityCD : MonoBehaviour
{


    [SerializeField]
    private Image imageCD;
    [SerializeField]
    private TMP_Text textCD;
    [SerializeField]
    Slider slider;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        textCD.gameObject.SetActive(false);
        imageCD.fillAmount = 0;
        GameEvents.instance.OnInvisibilityCooldown += StartCooldown;
        GameEvents.instance.OnInvisibility += StartDuration;
        slider.value = 0;
    }

    private void OnDestroy()
    {
        GameEvents.instance.OnInvisibilityCooldown -= StartCooldown;
        GameEvents.instance.OnInvisibility -= StartDuration;
    }

    void StartCooldown(float cooldown)
    {
        StartCoroutine(Cooldown(cooldown));
    }

    void StartDuration(float duration)
    {
        StartCoroutine(Duration(duration));
    }

    IEnumerator Cooldown(float cooldown)
    {
        Debug.Log("here 10");
        textCD.gameObject.SetActive(true);
        imageCD.fillAmount = 1;

        float timePassed = 0;
        Debug.Log("here11");
        while (timePassed < cooldown)
        {
            textCD.text = Mathf.RoundToInt(cooldown - timePassed).ToString();
            imageCD.fillAmount = (cooldown - timePassed) / cooldown;
            timePassed += Time.deltaTime;
            yield return null;
        }

        Debug.Log("here 12");
        imageCD.fillAmount = 0;
        textCD.gameObject.SetActive(false);

    }

    IEnumerator Duration(float duration)
    {
        imageCD.fillAmount = 1;
        slider.value = 1;
        
        float timePassed = 0;

        while(timePassed < duration)
        {
            slider.value = slider.value = (duration - timePassed) / duration;
            timePassed += Time.deltaTime;
            yield return null;
        }

        slider.value = 0;
    }

}
