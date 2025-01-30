using UnityEngine;

public class Bookshelf : MonoBehaviour
{
    [SerializeField]
    Rigidbody2D book;
    [SerializeField]
    Vector2 throwDirection;
    [SerializeField]
    GameObject hitTrigger;


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

        if(gameObject == this.gameObject)
        {
            // Instantiate Book
            Rigidbody2D bookInstance;
            bookInstance = Instantiate(book, this.gameObject.transform) as Rigidbody2D;
            bookInstance.AddForce(throwDirection * 350f);

            // Instantiate Hitbox
            Instantiate(hitTrigger);
        }

    }

}
