using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Finish : MonoBehaviour
{
    public GameObject son;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Sonner sonner = collision.GetComponent<Sonner>();
            sonner.Respawn();
        }
    }
}
