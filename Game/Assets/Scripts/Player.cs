using UnityEngine;

namespace Game
{
    public class Player : MonoBehaviour
    {
        public PlayerMovementAdvanced playerMovementScript;
        public PlayerCam playerCameraScript;
        public float maxInteractionDistance;

        [Header("Private properties, exposed for debugging")]
        public Interactable currentInteraction;

        private void OnEnable()
        {
            if (currentInteraction != null)
            {
                currentInteraction.OnInteractionEnd += OnInteractionEnd;
            }
        }

        private void OnDisable()
        {
            if (currentInteraction != null)
            {
                currentInteraction.OnInteractionEnd -= OnInteractionEnd;
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Interact();
            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (currentInteraction != null)
                {
                    currentInteraction.EndInteraction();
                }
            }
        }

        public void Lock()
        {
            playerCameraScript.enabled = false;
            playerMovementScript.enabled = false;
        }

        public void Unlock()
        {
            playerCameraScript.enabled = true;
            playerMovementScript.enabled = true;
        }

        private void Interact()
        {
            var ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            if (Physics.Raycast(ray, out var hit, maxInteractionDistance))
            {
                GameObject hitGameObject = hit.collider.gameObject;
                var interactable = hitGameObject.GetComponent<Interactable>();

                if (interactable != null)
                {
                    Lock();
                    playerCameraScript.LookAt(interactable.interactionCameraTarget);
                    interactable.StartInteraction();
                    currentInteraction = interactable;
                    interactable.OnInteractionEnd += OnInteractionEnd;
                }
            }
        }

        private void OnInteractionEnd()
        {
            Unlock();
            currentInteraction = null;
        }
    }
}
