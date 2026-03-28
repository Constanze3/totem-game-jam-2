using System;
using System.Collections;
using System.Runtime.Serialization;
using System.Text;
using TMPro;
using UnityEngine;

//using System.Diagnostics;

public class TVController : MonoBehaviour
{
    public static TVController Instance;

    private StringBuilder inputBuffer = new StringBuilder();
    private Coroutine inputCoroutine;

    [SerializeField]
    private float inputDelay = 1.0f;

    [SerializeField]
    private TMP_Text displayText;

    [SerializeField]
    private Person[] people;

    [SerializeField]
    private int channelCodeLength = 4;

    private int currentPersonIndex = 0;

    private string correctChannelCode = "";

    [SerializeField]
    private Texture2D[] channelTextures;

    private Renderer screenRenderer;

    void Awake()
    {
        Instance = this;
    }

    void OnEnable()
    {
        Remote.OnRemoteButtonPressed += HandleButton;
    }

    void Start()
    {
        screenRenderer = GetComponent<Renderer>();

        correctChannelCode = GenerateChannelCode(channelCodeLength);

        for (int i = 0; i < people.Length; i++)
        {
            if (i != currentPersonIndex)
            {
                people[i].satisfied = true;
            }
        }

        screenRenderer.material.mainTexture = channelTextures[0];
    }

    void OnDisable()
    {
        Remote.OnRemoteButtonPressed -= HandleButton;
    }

    void HandleButton(string buttonName)
    {
        inputBuffer.Append(buttonName);

        displayText.text = inputBuffer.ToString();

        // Restart timeout coroutine
        if (inputCoroutine != null)
            StopCoroutine(inputCoroutine);

        inputCoroutine = StartCoroutine(InputTimeout());
    }

    IEnumerator InputTimeout()
    {
        yield return new WaitForSeconds(inputDelay);

        CommitChannel();
    }

    void CommitChannel()
    {
        if (inputBuffer.Length == 0)
        {
            return;
        }

        displayText.text = "";

        if (inputBuffer.ToString() == correctChannelCode)
            ChangeChannel();

        inputBuffer.Clear();
        inputCoroutine = null;
    }

    void ChangeChannel()
    {
        // TODO: Implement visuals and channel switching logic here
        Debug.Log("Switching to channel: " + correctChannelCode);

        int newImageIndex = 0;

        while (newImageIndex == Array.IndexOf(channelTextures, screenRenderer.material.mainTexture))
        {
            newImageIndex = UnityEngine.Random.Range(0, channelTextures.Length);
        }

        screenRenderer.material.mainTexture = channelTextures[newImageIndex];

        people[currentPersonIndex].satisfied = true;
        people[currentPersonIndex].resetRage();
        currentPersonIndex = (currentPersonIndex + 1) % people.Length;
        Debug.Log("New dissatisfied person: " + ((currentPersonIndex + 1) % people.Length));
        people[currentPersonIndex].satisfied = false;
        correctChannelCode = GenerateChannelCode(channelCodeLength);
    }

    String GenerateChannelCode(int length)
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
