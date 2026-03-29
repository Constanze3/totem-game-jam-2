using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PopupBehavior : MonoBehaviour
{
    public Text messageText;
    public Button closeButton;
    public Image popupImage;

    public bool wandersAround = true;
    public float moveSpeed = 80f;
    private Vector2 moveDir;
    private RectTransform rt;

    void Start()
    {
        rt = GetComponent<RectTransform>();
        moveDir = Random.insideUnitCircle.normalized;

        bool fakeClose = Random.value > 0.5f;
        closeButton.onClick.AddListener(() => {
            if (fakeClose)
                SpawnTwoMore();
            else
                Destroy(gameObject);
        });

        StartCoroutine(ShakeRoutine());
    }

    void Update()
    {
        if (!wandersAround) return;
        rt.anchoredPosition += moveDir * moveSpeed * Time.deltaTime;

        if (Mathf.Abs(rt.anchoredPosition.x) > 600)
            moveDir.x *= -1;
        if (Mathf.Abs(rt.anchoredPosition.y) > 350)
            moveDir.y *= -1;
    }

    public void SetMessage(string msg)
    {
        if (messageText) messageText.text = msg;
    }

    public void SetSprite(Sprite sprite)
    {
        if (popupImage != null)
        {
            popupImage.sprite = sprite;
            popupImage.enabled = true;
        }
    }

    void SpawnTwoMore()
    {
        var spawner = FindFirstObjectByType<PopupSpawner>();
        if (spawner) { spawner.SpawnPopup(); spawner.SpawnPopup(); }
        Destroy(gameObject);
    }

    IEnumerator ShakeRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(2f, 5f));
            for (int i = 0; i < 10; i++)
            {
                rt.anchoredPosition += (Vector2)Random.insideUnitCircle * 6f;
                yield return new WaitForSeconds(0.04f);
            }
        }
    }
}