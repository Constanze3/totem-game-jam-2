using System;
using Game;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public Transform interactionCameraTarget;
    public AudioClip audioClip;
    public AudioSource audioSource;

    [Header("Private properties, exposed for debugging")]
    public bool inInteraction;

    public event Action OnInteractionStart;
    public event Action OnInteractionEnd;

    public void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void StartInteraction()
    {
        if (inInteraction)
        {
            return;
        }

        Debug.Log("Start interaction");

        inInteraction = true;

        audioSource.clip = audioClip;
        audioSource.Play();

        OnInteractionStart?.Invoke();
    }

    public void EndInteraction()
    {
        if (!inInteraction)
        {
            return;
        }

        Debug.Log("Stop interaction");

        audioSource.Stop();
        OnInteractionEnd?.Invoke();

        inInteraction = false;
    }
}
