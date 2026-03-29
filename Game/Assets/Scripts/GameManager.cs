using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject endScreenPanel;

    public TextMeshProUGUI logsTMP;

    [Header("Time Settings")]
    public float startTime = 9f; // start at 9:00
    public float endTime = 19f; // end at 19:00
    public float dayDuration = 180f; // real seconds for full day

    public float currentTime = 9f;

    private bool gameEnded = false;

    [Header("Logs Settings")]
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
            template = "{name} became {state} after being stuck at Frogger",
        },
        new SentenceTemplate
        {
            activity = Person.Activity.Frogger,
            template = "{name} got run over in frogger one too many times and became {state}",
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
            template = "{name} didn't manage to get a new onlyfans subscription so he's {state}",
        },
        new SentenceTemplate
        {
            activity = Person.Activity.CardSwipe,
            template =
                "{name} got {state} after not being able to pay for his daughter's flight back from India",
        },
    };

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
        GameObject[] personObjects = GameObject.FindGameObjectsWithTag("Person");
        UnityEngine.Debug.Log("People: " + personObjects.Length);

        finalScore = startingScore;

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

        UnityEngine.Debug.Log("Day finished!");

        ShowScoreScreen();
    }

    void ShowScoreScreen()
    {
        // Enable UI panel
        endScreenPanel.SetActive(true);

        // Display logs
        string fullLog = "";

        foreach (string log in logs)
        {
            fullLog += log;
        }

        logsTMP.text = fullLog;

        logsTMP.text += "---------------------------------------------------------\n";

        // Calculate score

        logsTMP.text += "Final Score: " + finalScore;
    }

    void HandlePersonStateChanged(Person person, Person.State newState)
    {
        UnityEngine.Debug.Log($"{person.personName} changed to {newState}");

        if (newState != Person.State.Idle)
            logs.Add(GenerateSentence(person, person.currentState, person.currentActivity));
    }

    string GenerateSentence(Person person, Person.State state, Person.Activity activity)
    {
        // Filter by activity
        List<SentenceTemplate> validTemplates = templates.FindAll(t => t.activity == activity);

        if (validTemplates.Count == 0)
        {
            UnityEngine.Debug.Log(
                $"{person.personName} doing {activity} in state {state} generated an empty log"
            );
            return "";
        }

        var template = validTemplates[Random.Range(0, validTemplates.Count)].template;

        string result = "(" + GetFormattedTime() + ") ";

        result += template
            .Replace("{name}", person.personName)
            .Replace("{state}", state.ToString().ToLower());

        if (state == Person.State.Annoyed)
        {
            result += " (-" + annoyedPenalty.ToString() + ")";
            finalScore -= annoyedPenalty;
        }
        else if (state == Person.State.Angry)
        {
            result += " (-" + angryPenalty.ToString() + ")";
            finalScore -= angryPenalty;
        }
        else if (state == Person.State.Raging)
        {
            result += " (-" + ragingPenalty.ToString() + ")";
            finalScore -= ragingPenalty;
        }

        return result + "\n";
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
