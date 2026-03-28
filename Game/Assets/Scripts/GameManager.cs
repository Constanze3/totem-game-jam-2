using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Time Settings")]
    public float currentTime = 9f; // start at 9:00
    public float endTime = 19f; // end at 19:00
    public float dayDuration = 180f; // real seconds for full day

    private bool gameEnded = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (!gameEnded)
        {
            UpdateTime();
        }
    }

    void UpdateTime()
    {
        float hoursPerSecond = (endTime - currentTime) / dayDuration;

        currentTime += hoursPerSecond * Time.deltaTime;

        Debug.Log("Current time: " + GetFormattedTime());

        if (currentTime >= endTime)
        {
            currentTime = endTime;
            EndGame();
        }
    }

    void EndGame()
    {
        gameEnded = true;

        Debug.Log("Day finished!");

        ShowScoreScreen();
    }

    void ShowScoreScreen()
    {
        // TODO:
        // - Enable UI panel
        // - Calculate score
        
        // Pause game
        Time.timeScale = 0f;
    }

    public float GetNormalizedTime()
    {
        return (currentTime - 9f) / (endTime - 9f);
    }

    public string GetFormattedTime()
    {
        int hours = Mathf.FloorToInt(currentTime);
        int minutes = Mathf.FloorToInt((currentTime - hours) * 60);

        return $"{hours:00}:{minutes:00}";
    }
}
