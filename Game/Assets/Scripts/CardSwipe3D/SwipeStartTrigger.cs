using UnityEngine;

public class SwipeStartTrigger: MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<SwipeManager>(out var card))
        {
            card.StartSwipe();
        }
    }
}
