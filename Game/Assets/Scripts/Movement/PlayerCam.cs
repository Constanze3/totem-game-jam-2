using System.Collections;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    public float sensX;
    public float sensY;

    public Transform orientation;
    public Transform camHolder;
    private Quaternion savedRotation;
    private Vector3 savedPosition;
    private bool transformSaved = false;
    float xRotation;
    float yRotation;

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (transformSaved)
        {
            transform.position = savedPosition;
            transform.localRotation = Quaternion.Euler(0, 0, 0);
            transformSaved = false;
        }
    }

    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        savedPosition = transform.position;
        savedRotation = transform.rotation;
        transformSaved = true;
    }

    private void Update()
    {
        // get mouse input
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        yRotation += mouseX;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // rotate cam and orientation
        camHolder.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    public void DoFov(float endValue)
    {
        GetComponent<Camera>().DOFieldOfView(endValue, 0.25f);
    }

    public void DoTilt(float zTilt)
    {
        transform.DOLocalRotate(new Vector3(0, 0, zTilt), 0.25f);
    }

    public IEnumerator SmoothMoveCamera(
        Vector3 toPosition,
        Quaternion toRotation,
        float movementSpeed,
        float rotationSpeed
    )
    {
        while (Vector3.Distance(transform.position, toPosition) > 0.01)
        {
            // Exponential decay
            var position = Vector3.Lerp(transform.position, toPosition, movementSpeed);
            transform.position = position;

            // Exponential decay
            var rotation = Quaternion.Slerp(transform.rotation, toRotation, rotationSpeed);
            transform.rotation = rotation;

            yield return null;
        }

        transform.position = toPosition;
        transform.rotation = toRotation;
    }
}
