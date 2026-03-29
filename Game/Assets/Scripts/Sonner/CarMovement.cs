using UnityEngine;

public class Carmovement : MonoBehaviour
{
    public Vector2 direction = Vector2.right;
    public float speed = 1f;
    public int size = 1;

    public Sonner sonnerGame;

    private void Update()
    {
        var movingRight = 0 < direction.x;
        var movingLeft = direction.x < 0;

        if (movingRight && sonnerGame.rightEdge < transform.position.x - size)
        {
            transform.position = new Vector3(
                sonnerGame.leftEdge - size,
                transform.position.y,
                transform.position.z
            );
        }
        else if (movingLeft && transform.position.x + size < sonnerGame.leftEdge)
        {
            transform.position = new Vector3(
                sonnerGame.rightEdge + size,
                transform.position.y,
                transform.position.z
            );
        }
        else
        {
            transform.Translate(speed * Time.deltaTime * direction);
        }
    }
}
