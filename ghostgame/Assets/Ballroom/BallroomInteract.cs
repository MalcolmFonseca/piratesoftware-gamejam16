using UnityEngine;

public class BallroomInteract : MonoBehaviour
{
    [SerializeField]
    Rigidbody2D plate;
    [SerializeField]
    Rigidbody2D wine;
    [SerializeField]
    Vector2 throwDirection;


    private void Start()
    {
        // subscribe
        GameEvents.instance.onInteract += ThrowBook;
    }

    private void OnDestroy()
    {
        // unsubscribe
        GameEvents.instance.onInteract -= ThrowBook;
    }

    private void ThrowBook(GameObject gameObject)
    {

        if (gameObject == this.gameObject)
        {

            float option = Random.Range(0, 5);
            if(option > 2.5f)
            {
                Rigidbody2D plateInstance;
                plateInstance = Instantiate(plate, this.gameObject.transform) as Rigidbody2D;
                plateInstance.AddForce(throwDirection * 550f);
            } else
            {
                Rigidbody2D wineInstance;
                wineInstance = Instantiate(wine, this.gameObject.transform) as Rigidbody2D;
                wineInstance.AddForce(throwDirection * 550f);
            }
            
        }

    }
}
