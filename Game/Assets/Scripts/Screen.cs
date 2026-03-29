using System.Collections;
using UnityEngine;

public class Screen : MonoBehaviour
{
    public Interactable interactable;
    public string sceneToLoadOnScreen;
    public Material materialTheCameraInLoadedSceneRendersTo;
    public Material screenOffMaterial;

    [Header("Private properties, exposed for debugging")]
    public new Renderer renderer;

    private void OnEnable()
    {
        interactable.OnInteractionStart += ShowScreen;
        interactable.OnInteractionEnd += ClearScreen;
    }

    private void OnDisable()
    {
        interactable.OnInteractionStart -= ShowScreen;
        interactable.OnInteractionEnd -= ClearScreen;
    }

    private void Awake()
    {
        renderer = GetComponent<Renderer>();
    }

    public void ShowScreen()
    {
        Debug.Log("Loading screen");

        var scene = SceneManager.Instance.GetSceneByName(sceneToLoadOnScreen);

        IGame game = null;
        foreach (var gameObject in scene.GetRootGameObjects())
        {
            if (gameObject.CompareTag("Game"))
            {
                game = gameObject.GetComponent<IGame>();
            }
        }

        if (game == null)
        {
            Debug.LogError(
                "A game object with tag 'Game' should be at the root of the loaded scene"
            );
        }

        renderer.material = materialTheCameraInLoadedSceneRendersTo;
        game.SetInteractable(interactable);
    }

    public void ClearScreen()
    {
        Debug.Log("Clearing screen");

        renderer.material = screenOffMaterial;
    }
}
