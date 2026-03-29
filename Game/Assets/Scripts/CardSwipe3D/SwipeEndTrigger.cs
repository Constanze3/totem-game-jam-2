using UnityEngine;

public class SwipeEndTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<SwipeManager>(out var card))
        {
            card.EndSwipe();
        }
    }
}
