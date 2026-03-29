using Game;
using TMPro;
using UnityEngine;

public class Sonner : MonoBehaviour, IGame
{
    public Camera gameCamera;

    public Son son;
    public Transform cars;

    public float leftEdge;
    public float rightEdge;

    public Interactable interactable;
    public Person person;

    private void Awake()
    {
        AttachSonnerToChildrenRecursively(cars);
    }

    private void AttachSonnerToChildrenRecursively(Transform parent)
    {
        foreach (Transform child in parent)
        {
            if (child.gameObject.TryGetComponent<Carmovement>(out var car))
            {
                car.sonnerGame = this;
            }

            AttachSonnerToChildrenRecursively(child);
        }
    }

    private void Start()
    {
        float halfWidth = gameCamera.orthographicSize * gameCamera.aspect;
        float camX = gameCamera.transform.position.x;

        leftEdge = camX - halfWidth;
        rightEdge = camX + halfWidth;
    }

    public void ProvideContext(Interactable interactable, Person person)
    {
        this.interactable = interactable;
        this.person = person;

        this.interactable.OnInteractionStart += () =>
        {
            son.gameObject.SetActive(true);
        };

        this.interactable.OnInteractionEnd += () =>
        {
            son.gameObject.SetActive(false);
        };
    }
}
