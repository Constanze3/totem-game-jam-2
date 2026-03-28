using UnityEngine;

namespace Game
{
    public class Interact : MonoBehaviour
    {
        [SerializeField] private Transform player;
        [SerializeField] private float interactDistance = 3f;
        [SerializeField] private Interactable interactor;

        private void Update()
        {
            if (Vector3.Distance(player.position, transform.position) <= interactDistance)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    interactor.Interact();
                }
            }
        }

    }
}
