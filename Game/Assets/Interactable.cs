using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Interactable : MonoBehaviour
{
    [Header("Scene Loading")]
    [SerializeField] private string sceneToLoad = "PhoneGameScene";

    [Header("Target")]
    [SerializeField] private Renderer targetRenderer;
    [SerializeField] private Material screenMaterial;

    private bool isLoading;
    private bool isLoaded;
    public event Action OnInteract;
    public void Interact()
    {
        if (isLoaded || isLoading)
            return;

        OnInteract?.Invoke();
    }
    //This load phone screen should be in the screenManager class.

    private IEnumerator LoadPhoneScene()
    {
        isLoading = true;

        AsyncOperation loadOp = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);

        while (!loadOp.isDone)
            yield return null;

        isLoaded = true;
        isLoading = false;
        ApplyMaterial();
    }

    private void ApplyMaterial()
    {
        if (targetRenderer == null || screenMaterial == null)
        {
            Debug.LogWarning("Target renderer or screen material is missing.");
            return;
        }

        targetRenderer.material = screenMaterial;
    }
}
