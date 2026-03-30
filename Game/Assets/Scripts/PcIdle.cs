using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PcIdle : MonoBehaviour
{
    public Interactable interactable;
    public AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        interactable.OnInteractionStart += OnInteractionStart;
        interactable.OnInteractionEnd += OnInteractionEnd;
    }

    private void OnDisable()
    {
        interactable.OnInteractionStart -= OnInteractionStart;
        interactable.OnInteractionEnd -= OnInteractionEnd;
    }

    private void OnInteractionStart()
    {
        audioSource.enabled = false;
    }

    private void OnInteractionEnd()
    {
        audioSource.enabled = true;
    }
}
