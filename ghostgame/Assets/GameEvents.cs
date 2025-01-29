using System;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    public static GameEvents instance;

    private void Awake()
    {
        instance = this;
    }

    // Controls interact cooldown and tells what is being interacted with (called within player)
    public event Action<GameObject> onInteract;
    public void Interact(GameObject gameObject)
    {
        onInteract?.Invoke(gameObject);
    }


    // Controls invisibility duration (called within player)
    public event Action<float> onInvisibility;
    public void StartInvisibility(float duration) 
    {
        onInvisibility?.Invoke(duration);
    }

    // Controls invisibility cooldown (called within player)
    public event Action<float> onInvisibilityCooldown;
    public void StartCooldown(float cooldown)
    {
        onInvisibilityCooldown?.Invoke(cooldown);
    }


    // Controls player taking damage (called within flashlight)
    // Subscribed to by healthbar UI and player
    public event Action onTakingDamage;
    public void TakeDamage()
    {
        onTakingDamage?.Invoke();
    }
}
