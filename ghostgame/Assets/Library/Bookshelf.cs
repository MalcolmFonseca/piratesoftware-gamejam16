using UnityEngine;

public class Bookshelf : MonoBehaviour
{
    [SerializeField]
    Rigidbody2D book;
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

        if(gameObject == this.gameObject)
        {
            //logic
            //instantiate with velocity
            Rigidbody2D bookInstance;
            bookInstance = Instantiate(book, this.gameObject.transform) as Rigidbody2D;
            bookInstance.AddForce(throwDirection * 350f);
        }

    }

}
