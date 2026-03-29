using System;
using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class TVController : MonoBehaviour
{
    public float inputDelay = 1.0f;
    public int correctChannelCodeLength = 4;
    public TMP_Text displayText;
    public GameObject remote;
    public Person[] people;
    public Texture2D[] channelTextures;

    private int unsatisfiedIndex = 0;
    private string correctChannelCode = "";
    private StringBuilder inputBuffer = new StringBuilder();
    private Coroutine inputTimeoutCoroutine;
    private int currentChannelTextureIndex = 0;

    private Renderer tvScreenRenderer;

    public float minHintDelay = 3f;

    public float maxHintDelay = 6f;

    void OnEnable()
    {
        Remote.OnRemoteButtonPressed += HandleButton;
    }

    void OnDisable()
    {
        Remote.OnRemoteButtonPressed -= HandleButton;
    }

    void Start()
    {
        tvScreenRenderer = GetComponent<Renderer>();
        ChangeChannel();
    }

    void HandleButton(string buttonName)
    {
        inputBuffer.Append(buttonName);
        displayText.text = inputBuffer.ToString();

        // Restart input timeout coroutine coroutine
        if (inputTimeoutCoroutine != null)
        {
            StopCoroutine(inputTimeoutCoroutine);
        }
        inputTimeoutCoroutine = StartCoroutine(InputTimeout());
    }

    IEnumerator InputTimeout()
    {
        yield return new WaitForSeconds(inputDelay);
        CommitChannelCode();
    }

    private void CommitChannelCode()
    {
        if (inputBuffer.Length == 0)
        {
            return;
        }

        displayText.text = "";

        if (inputBuffer.ToString() == correctChannelCode)
        {
            ChangeChannel();
        }

        inputBuffer.Clear();
        StopCoroutine(inputTimeoutCoroutine);
    }

    private void ChangeChannel()
    {
        Debug.Log("Switching to channel: " + correctChannelCode);

        var newChannelTextureIndex = currentChannelTextureIndex;
        while (newChannelTextureIndex == currentChannelTextureIndex)
        {
            newChannelTextureIndex = UnityEngine.Random.Range(0, channelTextures.Length);
        }

        tvScreenRenderer.materials[1].mainTexture = channelTextures[newChannelTextureIndex];

        people[unsatisfiedIndex].startingRageRate = people[unsatisfiedIndex].rageRate; // Makes sure to save previous rage rate to revert back to

        people[unsatisfiedIndex].rageRate = 0f;
        people[unsatisfiedIndex].SetRage(0f);

        unsatisfiedIndex = (unsatisfiedIndex + 1) % people.Length;
        people[unsatisfiedIndex].rageRate = people[unsatisfiedIndex].startingRageRate;

        correctChannelCode = GenerateChannelCode(correctChannelCodeLength);

        remote.GetComponent<Interactable>().EndInteraction();

        // Immediately show a hint for the new channel code
        StopAllCoroutines();
        StartCoroutine(ShowHint());
    }

    private String GenerateChannelCode(int length)
    {
        StringBuilder code = new StringBuilder();

        for (int i = 0; i < length; i++)
        {
            int digit = UnityEngine.Random.Range(0, 9);
            code.Append(digit);
        }

        return code.ToString();
    }

    IEnumerator ShowHint()
    {
        Person currentPerson = people[unsatisfiedIndex];

        string message = $"I need to watch channel {correctChannelCode}!!";

        currentPerson.Say(message);

        float randomDelay = UnityEngine.Random.Range(minHintDelay, maxHintDelay);

        yield return new WaitForSeconds(randomDelay);

        StartCoroutine(ShowHint());
    }
}
