using TMPro;
using UnityEngine;

public class Clock : MonoBehaviour
{
    private float elapsedTime = 72000f;
    private TMP_Text clockText;
    private float timeScale = 20f;

    void Start()
    {
        clockText = GetComponent<TMP_Text>();
    }

    void Update()
    {
        elapsedTime += Time.deltaTime * timeScale;

        //update text
        int hours = Mathf.FloorToInt(elapsedTime/3600f);
        int minutes = Mathf.FloorToInt((elapsedTime - hours*3600f)/60f);

        //enforce clock cycle
        if(elapsedTime >= 86400f)
        {
            elapsedTime = 0;
        }

        string clockString = string.Format("{0:00}:{1:00}", hours, minutes);
        clockText.text = clockString;
    }
}
