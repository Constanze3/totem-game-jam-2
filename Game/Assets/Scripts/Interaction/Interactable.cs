using System;
using System.Collections;
using Game;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Interactable : MonoBehaviour
{
    public Transform inInteractionCameraTransform;
    public float interactionCameraAnimationMovementSpeed;
    public float interactionCameraAnimationRotationSpeed;

    public AudioSource audioSource;
    public AudioClip interactionStartAudioClip;
    public AudioClip interactionEndAudioClip;

    [TextArea]
    public string hoverHintText;

    [TextArea]
    public string inInteractionHintText;

    [Header("Private properties, exposed for debugging")]
    public Vector3 originalCameraPosition;
    public Quaternion originalCameraRotation;
    public bool inInteraction;
    public PlayerCam playerCameraScript;
    public Coroutine cameraAnimation;
    public HintManager hintManager;

    public event Action OnInteractionStart;
    public event Action OnInteractionEnd;

    public void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Start()
    {
        hintManager = GameManager.Instance.hintManager;
        playerCameraScript = GameManager.Instance.player.playerCameraScript;
    }

    public void StartInteraction()
    {
        if (inInteraction)
        {
            return;
        }

        Debug.Log("Start interaction");

        inInteraction = true;

        hintManager.ShowHint(inInteractionHintText);

        if (interactionStartAudioClip != null)
        {
            audioSource.clip = interactionStartAudioClip;
            audioSource.Play();
        }

        originalCameraPosition = playerCameraScript.gameObject.transform.position;
        originalCameraRotation = playerCameraScript.gameObject.transform.rotation;
        cameraAnimation = StartCoroutine(
            playerCameraScript.SmoothMoveCamera(
                inInteractionCameraTransform.position,
                inInteractionCameraTransform.rotation,
                interactionCameraAnimationMovementSpeed,
                interactionCameraAnimationRotationSpeed
            )
        );

        OnInteractionStart?.Invoke();
    }

    public void EndInteraction()
    {
        if (!inInteraction)
        {
            return;
        }

        inInteraction = false;

        Debug.Log("Interaction ended");

        hintManager.HideHint();

        audioSource.Stop();
        if (interactionEndAudioClip != null)
        {
            audioSource.clip = interactionEndAudioClip;
            audioSource.Play();
        }

        if (cameraAnimation != null)
        {
            StopCoroutine(cameraAnimation);
        }
        cameraAnimation = StartCoroutine(EndInteractionCoroutine());
    }

    private IEnumerator EndInteractionCoroutine()
    {
        yield return StartCoroutine(
            playerCameraScript.SmoothMoveCamera(
                originalCameraPosition,
                originalCameraRotation,
                interactionCameraAnimationMovementSpeed,
                interactionCameraAnimationRotationSpeed
            )
        );

        OnInteractionEnd?.Invoke();
    }
}
