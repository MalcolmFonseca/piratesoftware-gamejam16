using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CandleController : MonoBehaviour
{

    Light2D light;
    Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        light = GetComponent<Light2D>();
    }

    private void OnEnable()
    {
        PlayerMovement.OnInteract += ToggleLight;
    }

    private void OnDisable()
    {
        PlayerMovement.OnInteract -= ToggleLight;
    }

    private void ToggleLight(PlayerMovement player)
    {
        light.enabled = !light.enabled;
        anim.SetBool("isFlame", light.enabled);
    }

}
