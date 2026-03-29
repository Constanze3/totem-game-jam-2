using UnityEngine;

public class CardSlotTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Hit: " + other.gameObject.name);  // ← added this
        if (other.TryGetComponent<CardSwipe>(out var card))
            card.EnterSlot();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent<CardSwipe>(out var card))
            card.ExitSlot();
    }
}