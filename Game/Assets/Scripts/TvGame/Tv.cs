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
    public Person[] people;
    public Texture2D[] channelTextures;

    private int unsatisfiedIndex = 0;
    private string correctChannelCode = "";
    private StringBuilder inputBuffer = new StringBuilder();
    private Coroutine inputTimeoutCoroutine;
    private int currentChannelTextureIndex = 0;

    private Renderer tvScreenRenderer;

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
        Debug.Log(
            "New dissatisfied person: "
                + people[unsatisfiedIndex]
                + " is now raging with rate: "
                + people[unsatisfiedIndex].rageRate
        );

        correctChannelCode = GenerateChannelCode(correctChannelCodeLength);
    }

    private String GenerateChannelCode(int length)
    {
        StringBuilder code = new StringBuilder();

        for (int i = 0; i < length; i++)
        {
            int digit = UnityEngine.Random.Range(0, 9); // TODO: Implement more buttons
            code.Append(digit);
        }

        Debug.Log("New channel code: " + code.ToString());
        return code.ToString();
    }
}
