using System.Collections.Generic;
using System.Text;
using Game;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Player player;

    [Header("UI")]
    public GameObject endScreenPanel;
    public TextMeshProUGUI logsTMP;

    [Header("Time Settings")]
    public float startTime = 9f;
    public float endTime = 19f;
    public float dayDuration = 180f;

    public float currentTime = 9f;

    private bool gameEnded = false;

    [Header("Score Settings")]
    public int annoyedPenalty;
    public int angryPenalty;
    public int ragingPenalty;
    public int startingScore;

    private int finalScore;

    private List<string> logs = new List<string>();

    [System.Serializable]
    public class SentenceTemplate
    {
        public Person.Activity activity;
        public string template;
    }

    [SerializeField]
    private List<SentenceTemplate> templates = new List<SentenceTemplate>()
    {
        new SentenceTemplate
        {
            activity = Person.Activity.Tv,
            template = "{name} got {state} because they couldn't watch Mr Beast",
        },
        new SentenceTemplate
        {
            activity = Person.Activity.Tv,
            template = "{name} is {state} after failing to watch enough brainrot",
        },
        new SentenceTemplate
        {
            activity = Person.Activity.Frogger,
            template = "{name} became {state} after being stuck at Sonner",
        },
        new SentenceTemplate
        {
            activity = Person.Activity.Frogger,
            template = "{name} got run over in Sonner one too many times and became {state}",
        },
        new SentenceTemplate
        {
            activity = Person.Activity.CloseAds,
            template = "{name} is {state} from getting overwhelmed by ads",
        },
        new SentenceTemplate
        {
            activity = Person.Activity.CloseAds,
            template = "{name} is {state} because of the neverending stream of ads",
        },
        new SentenceTemplate
        {
            activity = Person.Activity.CardSwipe,
            template = "{name} is {state} since he couldn't buy NFT's",
        },
        new SentenceTemplate
        {
            activity = Person.Activity.CardSwipe,
            template = "{name} didn't manage to get a new subscription so he's {state}",
        },
        new SentenceTemplate
        {
            activity = Person.Activity.CardSwipe,
            template = "{name} got {state} after failing to pay for something important",
        },
    };

    // Cached lookup
    private Dictionary<Person.Activity, List<SentenceTemplate>> templateMap;

    public GameObject speechBubblePrefab;

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

    void Start()
    {
        finalScore = startingScore;

        // Build template lookup
        templateMap = new Dictionary<Person.Activity, List<SentenceTemplate>>();

        foreach (var template in templates)
        {
            if (!templateMap.ContainsKey(template.activity))
                templateMap[template.activity] = new List<SentenceTemplate>();

            templateMap[template.activity].Add(template);
        }

        // Subscribe to people
        GameObject[] personObjects = GameObject.FindGameObjectsWithTag("Person");

        Debug.Log("People: " + personObjects.Length);

        foreach (GameObject obj in personObjects)
        {
            Person person = obj.GetComponent<Person>();

            if (person != null)
            {
                person.OnStateChanged += HandlePersonStateChanged;
            }
        }
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
        float hoursPerSecond = (endTime - startTime) / dayDuration;
        currentTime += hoursPerSecond * Time.deltaTime;

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
        endScreenPanel.SetActive(true);

        StringBuilder sb = new StringBuilder();

        foreach (string log in logs)
        {
            sb.AppendLine(log);
        }

        sb.AppendLine("------------------------------------");
        sb.AppendLine("Final Score: " + finalScore);

        logsTMP.text = sb.ToString();

        Time.timeScale = 0f;
    }

    void HandlePersonStateChanged(Person person, Person.State newState)
    {
        Debug.Log($"{person.personName} changed to {newState}");

        if (newState == Person.State.Idle)
            return;

        string sentence = GenerateSentence(person, newState, person.currentActivity);

        int penalty = GetPenalty(newState);
        finalScore -= penalty;

        string formattedLog = $"({GetFormattedTime()}) {sentence} (-{penalty})";

        logs.Add(formattedLog);
    }

    string GenerateSentence(Person person, Person.State state, Person.Activity activity)
    {
        if (!templateMap.ContainsKey(activity))
        {
            Debug.LogWarning($"{activity} has no templates");
            return "";
        }

        var list = templateMap[activity];
        var template = list[Random.Range(0, list.Count)].template;

        return template
            .Replace("{name}", person.personName)
            .Replace("{state}", state.ToString().ToLower());
    }

    int GetPenalty(Person.State state)
    {
        return state switch
        {
            Person.State.Annoyed => annoyedPenalty,
            Person.State.Angry => angryPenalty,
            Person.State.Raging => ragingPenalty,
            _ => 0,
        };
    }

    public float GetNormalizedTime()
    {
        return (currentTime - startTime) / (endTime - startTime);
    }

    public string GetFormattedTime()
    {
        int hours = Mathf.FloorToInt(currentTime);
        int minutes = Mathf.FloorToInt((currentTime - hours) * 60);

        return $"{hours:00}:{minutes:00}";
    }

    public void ShowSpeech(Transform personTransform, string message)
    {
        GameObject bubbleObj = Instantiate(
            speechBubblePrefab,
            personTransform.position,
            Quaternion.identity
        );

        SpeechBubble bubble = bubbleObj.GetComponent<SpeechBubble>();
        bubble.Init(personTransform, message);
    }
}
