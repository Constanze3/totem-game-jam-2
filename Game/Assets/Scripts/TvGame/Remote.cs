using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RemoteButton
{
    public string name;
    public GameObject buttonObject;

    public RemoteButton(string name, GameObject buttonObject)
    {
        this.name = name;
        this.buttonObject = buttonObject;
    }
}

public class Remote : MonoBehaviour
{
    public Camera cam;
    public float range = 5f;

    public List<RemoteButton> buttonList = new List<RemoteButton>();
    public static event Action<string> OnRemoteButtonPressed;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, range))
            {
                GameObject hitObject = hit.collider.gameObject;

                foreach (var button in buttonList)
                {
                    if (button.buttonObject == hitObject)
                    {
                        OnRemoteButtonPressed?.Invoke(button.name);

                        break;
                    }
                }
            }
        }
    }
}
