using System.Collections;
using TMPro;
using UnityEngine;

public class SpeechBubble : MonoBehaviour
{
    public TextMeshProUGUI text;

    public float duration = 2.5f;
    public float popInDuration = 0.2f;
    public float popOutDuration = 0.2f;

    private Transform target;
    private Camera mainCamera;

    void Awake()
    {
        mainCamera = Camera.main;
    }

    public void Init(Transform followTarget, string message)
    {
        target = followTarget;
        text.text = message;

        transform.localScale = Vector3.zero;

        gameObject.SetActive(true);

        StartCoroutine(PlayAnimation());
    }

    void Update()
    {
        if (target == null) return;

        transform.position = target.position + Vector3.up * 2f;
        transform.forward = mainCamera.transform.forward;
    }

    IEnumerator PlayAnimation()
    {
        // POP IN
        yield return ScaleOverTime(Vector3.zero, Vector3.one, popInDuration);

        // WAIT
        yield return new WaitForSeconds(duration);

        // POP OUT
        yield return ScaleOverTime(Vector3.one, Vector3.zero, popOutDuration);

        Destroy(gameObject);
    }

    IEnumerator ScaleOverTime(Vector3 from, Vector3 to, float time)
    {
        float t = 0f;

        while (t < time)
        {
            t += Time.deltaTime;
            float normalized = t / time;

            // Smooth easing
            normalized = Mathf.SmoothStep(0f, 1f, normalized);

            transform.localScale = Vector3.Lerp(from, to, normalized);
            yield return null;
        }

        transform.localScale = to;
    }
}