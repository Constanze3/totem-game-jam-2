using System.Collections;
using UnityEngine;

public class Screen : MonoBehaviour
{
    public Interactable interactable;
    public string sceneToLoadOnScreen;

    public Renderer screenRenderer;
    public int screenMaterialIndex = 0;

    public Material materialTheCameraInLoadedSceneRendersTo;
    public Material screenOffMaterial;

    private void OnEnable()
    {
        interactable.OnInteractionStart += ShowScreen;
        interactable.OnInteractionEnd += ClearScreen;

        ClearScreen();
    }

    private void OnDisable()
    {
        interactable.OnInteractionStart -= ShowScreen;
        interactable.OnInteractionEnd -= ClearScreen;

        ClearScreen();
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

        var materials = screenRenderer.materials;
        materials[screenMaterialIndex] = materialTheCameraInLoadedSceneRendersTo;
        screenRenderer.materials = materials;

        game.SetInteractable(interactable);
    }

    public void ClearScreen()
    {
        Debug.Log("Clearing screen");

        var materials = screenRenderer.materials;
        materials[screenMaterialIndex] = screenOffMaterial;
        screenRenderer.materials = materials;

        GetComponent<Renderer>().material = screenOffMaterial;
    }
}
