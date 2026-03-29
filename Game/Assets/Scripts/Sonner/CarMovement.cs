using UnityEngine;

public class Carmovement : MonoBehaviour
{
    public Vector2 direction = Vector2.right;
    public float speed = 1f;
    public int size = 1;

    public Camera gameCamera;

    private float leftEdge;
    private float rightEdge;

    private void Start()
    {
        float halfWidth = gameCamera.orthographicSize * gameCamera.aspect;
        float camX = gameCamera.transform.position.x;

        leftEdge = camX - halfWidth;
        rightEdge = camX + halfWidth;
    }

    private void Update()
    {
        // Check if the object is past the right edge of the screen
        if (direction.x > 0 && (transform.position.x - size) > rightEdge)
        {
            transform.position = new Vector3(
                leftEdge - size,
                transform.position.y,
                transform.position.z
            );
        }
        // Check if the object is past the left edge of the screen
        else if (direction.x < 0 && (transform.position.x + size) < leftEdge)
        {
            transform.position = new Vector3(
                rightEdge + size,
                transform.position.y,
                transform.position.z
            );
        }
        // Move the object
        else
        {
            transform.Translate(speed * Time.deltaTime * direction);
        }
    }
}
