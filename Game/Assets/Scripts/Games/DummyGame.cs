using UnityEngine;

public class DummyGame : MonoBehaviour, IGame
{
    private Interactable interactable;

    public void ProvideContext(Interactable interactable, Person _person)
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
