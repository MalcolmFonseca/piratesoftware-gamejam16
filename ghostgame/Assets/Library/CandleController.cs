using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CandleController : MonoBehaviour
{
    Light2D light;
    Animator anim;
    public int id;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        light = GetComponent<Light2D>();
    }

    private void OnEnable()
    {
        GameEvents.instance.onInteract += ToggleLight;
    }

    private void OnDisable()
    {
        GameEvents.instance.onInteract -= ToggleLight;
    }

    private void ToggleLight(GameObject gameObject)
    {
        if (gameObject == this.gameObject)
        {
            light.enabled = !light.enabled;
            anim.SetBool("isFlame", light.enabled);
        }
    }

}
