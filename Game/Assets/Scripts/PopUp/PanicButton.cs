using UnityEngine;
using UnityEngine.UI;

public class PanicButton : MonoBehaviour
{
    public PopupSpawner spawner;

    public float moveSpeed = 200f;  // faster than popups!
    private Vector2 moveDir;
    private RectTransform rt;

    void Start()
    {
        rt = GetComponent<RectTransform>();
        moveDir = Random.insideUnitCircle.normalized;

        // Start in center of screen
        rt.anchoredPosition = Vector2.zero;

        Button btn = GetComponent<Button>();
        if (btn) btn.onClick.AddListener(OnClicked);
    }

    void Update()
    {
        rt.anchoredPosition += moveDir * moveSpeed * Time.deltaTime;

        // Bounce off edges
        if (Mathf.Abs(rt.anchoredPosition.x) > 550)
            moveDir.x *= -1;
        if (Mathf.Abs(rt.anchoredPosition.y) > 300)
            moveDir.y *= -1;
    }

    void OnClicked()
    {
        if (spawner) spawner.OnPanicClicked();
        Destroy(gameObject);
    }
}