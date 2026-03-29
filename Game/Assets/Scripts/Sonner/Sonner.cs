using System.Collections;
using UnityEngine;

public class Sonner : MonoBehaviour
{
    public Sprite idleSprite;
    public Sprite leapSprite;
    public Sprite deadSprite;
    public Sprite wonSprite;

    private SpriteRenderer spriteRenderer;
    private Vector3 spawnPosition;
    private bool cooldown;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spawnPosition = transform.position;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            Rotate(0f);
            Move(Vector3.up);
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            Rotate(90f);
            Move(Vector3.left);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            Rotate(180f);
            Move(Vector3.down);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            Rotate(-90f);
            Move(Vector3.right);
        }
    }

    private void Move(Vector3 direction)
    {
        if (cooldown)
        {
            return;
        }

        Vector3 destination = transform.position + direction;

        Collider2D barrier = Physics2D.OverlapBox(
            destination,
            Vector2.zero,
            0f,
            LayerMask.GetMask("Barrier")
        );

        if (barrier != null)
        {
            return;
        }

        StartCoroutine(Leap(destination));
    }

    private void Rotate(float facing)
    {
        transform.rotation = Quaternion.Euler(0f, 0f, facing);
    }

    private IEnumerator Leap(Vector3 destination)
    {
        Vector3 startPosition = transform.position;

        float elapsed = 0f;
        float duration = 0.125f;

        spriteRenderer.sprite = leapSprite;
        cooldown = true;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            transform.position = Vector3.Lerp(startPosition, destination, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = destination;
        spriteRenderer.sprite = idleSprite;
        cooldown = false;
    }

    public void Respawn()
    {
        StopAllCoroutines();

        transform.SetPositionAndRotation(spawnPosition, Quaternion.identity);

        spriteRenderer.sprite = idleSprite;

        enabled = true;
        cooldown = false;
    }

    public void Death()
    {
        StopAllCoroutines();

        enabled = false;

        transform.rotation = Quaternion.identity;
        Debug.Log("Frog sprite");
        spriteRenderer.sprite = deadSprite;

        Invoke(nameof(Respawn), 1f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (enabled && collision.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            Death();
        }
    }
}
