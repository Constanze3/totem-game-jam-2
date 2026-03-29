using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Finish : MonoBehaviour
{
    public Son son;
    public Sonner sonner;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            sonner.interactable.EndInteraction();
            sonner.person.SetRage(0);
            son.Respawn();
        }
    }
}
