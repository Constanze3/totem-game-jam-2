using UnityEngine;

public class DummyGame : MonoBehaviour, IGame
{
    private Interactable interactable;

    public void SetInteractable(Interactable interactable)
    {
        this.interactable = interactable;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            interactable.EndInteraction();
        }
    }
}
