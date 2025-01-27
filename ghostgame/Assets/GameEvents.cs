using System;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    public static GameEvents instance;

    private void Awake()
    {
        instance = this;
    }

    public event Action<GameObject> onInteract;
    public void Interact(GameObject gameObject)
    {
        onInteract?.Invoke(gameObject);
    }

    public event Action<float> onStartCooldown;
    public void StartCooldown(float timer) 
    { 
        onStartCooldown?.Invoke(timer);
    }
}
