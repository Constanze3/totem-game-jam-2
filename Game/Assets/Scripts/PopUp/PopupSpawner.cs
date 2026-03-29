using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PopupSpawner : MonoBehaviour
{
    public GameObject popupPrefab;
    public GameObject panicButtonPrefab;
    public GameObject victoryText;
    public Canvas canvas;
    public Sprite[] sprites;

    public float spawnInterval = 1.5f;
    public int maxPopups = 10;
    private float timer;
    private bool panicSpawned = false;
    private bool gameOver = false;

    // Min/max size range for popups
    public float minWidth = 200f;
    public float maxWidth = 400f;
    public float minHeight = 150f;
    public float maxHeight = 300f;

    string[] messages = {
        "⚠️ VIRUS DETECTED! Click OK to fix!",
        "Your PC is infected with 47 viruses!",
        "CRITICAL ERROR: System32 corrupted!",
        "FREE OFFER: Click here to claim prize!",
        "WARNING: Your license has expired!",
    };

    void Start()
    {
        if (victoryText) victoryText.SetActive(false);
    }

    void Update()
    {
        if (gameOver) return;

        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            timer = 0f;
            if (transform.childCount < maxPopups)
                SpawnPopup();
        }

        if (!panicSpawned && transform.childCount >= maxPopups)
        {
            panicSpawned = true;
            SpawnPanicButton();
        }
    }

    public void SpawnPopup()
    {
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        float w = canvasRect.rect.width;
        float h = canvasRect.rect.height;

        GameObject popup = Instantiate(popupPrefab, transform);
        RectTransform rt = popup.GetComponent<RectTransform>();

        // Random size for each popup
        float randW = Random.Range(minWidth, maxWidth);
        float randH = Random.Range(minHeight, maxHeight);
        rt.sizeDelta = new Vector2(randW, randH);

        // Random position within screen bounds
        rt.anchoredPosition = new Vector2(
            Random.Range(-w / 2 + randW / 2, w / 2 - randW / 2),
            Random.Range(-h / 2 + randH / 2, h / 2 - randH / 2)
        );

        PopupBehavior pb = popup.GetComponent<PopupBehavior>();
        pb.SetMessage(messages[Random.Range(0, messages.Length)]);

        if (sprites != null && sprites.Length > 0)
            pb.SetSprite(sprites[Random.Range(0, sprites.Length)]);
    }

    void SpawnPanicButton()
    {
        if (panicButtonPrefab == null) return;
        GameObject panic = Instantiate(panicButtonPrefab, canvas.transform);
        panic.GetComponent<PanicButton>().spawner = this;
    }

    public void OnPanicClicked()
    {
        gameOver = true;

        foreach (Transform child in transform)
            Destroy(child.gameObject);

        StartCoroutine(ShowVictoryText());
    }

    IEnumerator ShowVictoryText()
    {
        if (victoryText == null) yield break;

        victoryText.SetActive(true);
        CanvasGroup cg = victoryText.GetComponent<CanvasGroup>();

        if (cg == null)
            cg = victoryText.AddComponent<CanvasGroup>();

        cg.alpha = 0f;

        while (cg.alpha < 1f)
        {
            cg.alpha += Time.deltaTime * 0.5f;
            yield return null;
        }

        cg.alpha = 1f;
    }
}