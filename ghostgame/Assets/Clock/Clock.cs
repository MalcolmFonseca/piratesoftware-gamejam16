using TMPro;
using UnityEngine;

public class Clock : MonoBehaviour
{
    private float elapsedTime = 72000f;
    private TMP_Text clockText;
    private float timeScale = 100f;
    private GameObject[] npcObjects;
    private float prevHour;
    private AudioSource source;
    [SerializeField]private AudioClip clip;

    void Start()
    {
        clockText = GetComponent<TMP_Text>();
        prevHour = Mathf.FloorToInt(elapsedTime / 3600f);
        source = GetComponent<AudioSource>();
    }

    void Update()
    {
        elapsedTime += Time.deltaTime * timeScale;

        //update text
        int hours = Mathf.FloorToInt(elapsedTime/3600f);
        int minutes = Mathf.FloorToInt((elapsedTime - hours*3600f)/60f);

        if (hours != prevHour) 
        {
            prevHour = hours;
            AlertHourChange();
        }

        //enforce clock cycle
        if(elapsedTime >= 86400f)
        {
            elapsedTime = 0;
        }

        string clockString = string.Format("{0:00}:{1:00}", hours, minutes);
        clockText.text = clockString;
    }

    private void AlertHourChange()
    {
        //play hour change sound
        source.PlayOneShot(clip);

        npcObjects = GameObject.FindGameObjectsWithTag("NPC");
        //trigger each npc to seek next room
        foreach (GameObject npcObject in npcObjects) 
        {
            FlightNPC npc = npcObject.GetComponent<FlightNPC>();
            if (npc != null)
            {
                npc.changePath();
            }
        }
    }
}
