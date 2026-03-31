using Unity.VisualScripting;
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
        public HintManager hintManager;
        public bool locked;

        private void OnEnable()
        {
            if (currentInteraction != null)
            {
                currentInteraction.OnInteractionEnd += OnInteractionEnd;
            }

            InvokeRepeating("Hover", 0f, 0.2f);
        }

        private void OnDisable()
        {
            if (currentInteraction != null)
            {
                currentInteraction.OnInteractionEnd -= OnInteractionEnd;
            }
        }

        private void Start()
        {
            hintManager = GameManager.Instance.hintManager;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButton((int)MouseButton.Right))
            {
                if (currentInteraction != null)
                {
                    currentInteraction.EndInteraction();
                }
            }

            if (locked)
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown((int)MouseButton.Left))
            {
                Interact();
            }
        }

        public void Lock()
        {
            locked = true;
            playerCameraScript.enabled = false;
            playerMovementScript.enabled = false;
        }

        public void Unlock()
        {
            locked = false;
            playerCameraScript.enabled = true;
            playerMovementScript.enabled = true;
        }

        private void Hover()
        {
            if (currentInteraction != null)
            {
                return;
            }

            var ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            if (Physics.Raycast(ray, out var hit, maxInteractionDistance))
            {
                GameObject hitGameObject = hit.collider.gameObject;
                var interactable = hitGameObject.GetComponent<Interactable>();

                if (interactable != null && !interactable.inInteraction)
                {
                    hintManager.ShowHint(interactable.hoverHintText);
                    return;
                }
            }

            hintManager.HideHint();
        }

        private void Interact()
        {
            var ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            if (Physics.Raycast(ray, out var hit, maxInteractionDistance))
            {
                GameObject hitGameObject = hit.collider.gameObject;
                var interactable = hitGameObject.GetComponent<Interactable>();

                if (interactable != null && !interactable.inInteraction)
                {
                    Lock();
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
