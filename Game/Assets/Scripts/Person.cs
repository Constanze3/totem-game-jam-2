using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class Person : MonoBehaviour
{
    public enum State
    {
        Idle,
        Annoyed,
        Angry,
        Raging,
    }

    public enum Activity
    {
        Tv,
        Frogger,
        CardSwipe,
        CloseAds,
    }

    public System.Action<Person, State> OnStateChanged;

    public string personName;
    public State currentState = State.Idle;
    public Activity currentActivity;

    [Header("Rage settings")]
    public float rage = 0f;
    public float rageRate = 1f;
    public float maxRage = 100f;

    public float annoyedThreshold = 25f;
    public float angryThreshold = 50f;
    public float ragingThreshold = 75f;

    [Header("Rage UI")]
    [SerializeField] private Slider rageSlider;
    [SerializeField] private Image fillImage;

    [Header("Rage Colors")]
    [SerializeField] private Color idleColor = Color.green;
    [SerializeField] private Color annoyedColor = Color.yellow;
    [SerializeField] private Color angryColor = new Color(1f, 0.5f, 0f);
    [SerializeField] private Color ragingColor = Color.red;

    private Animator animator;
    private AnimatorOverrideController overrideController;

    [Header("Animation Clips")]
    [SerializeField] private AnimationClip idleClip;
    [SerializeField] private AnimationClip annoyedClip;
    [SerializeField] private AnimationClip angryClip;
    [SerializeField] private AnimationClip ragingClip;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip idleAudioClip;
    [SerializeField] private AudioClip annoyedAudioClip;
    [SerializeField] private AudioClip angryAudioClip;
    [SerializeField] private AudioClip ragingAudioClip;

    public AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        animator = GetComponent<Animator>();

        overrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
        animator.runtimeAnimatorController = overrideController;

        UpdateAnimation();
        UpdateRageUI();
    }

    void Update()
    {
        IncreaseRageOverTime();
        UpdateRageUI();
    }

    void IncreaseRageOverTime()
    {
        SetRage(rage + rageRate * Time.deltaTime);
    }

    void UpdateRageUI()
    {
        if (rageSlider != null)
        {
            rageSlider.value = rage / maxRage;
        }

        if (fillImage != null)
        {
            if (rage >= ragingThreshold)
            {
                fillImage.color = ragingColor;
            }
            else if (rage >= angryThreshold)
            {
                fillImage.color = angryColor;
            }
            else if (rage >= annoyedThreshold)
            {
                fillImage.color = annoyedColor;
            }
            else
            {
                fillImage.color = idleColor;
            }
        }
    }

    public void SetState(State newState)
    {
        if (currentState == newState)
            return;

        currentState = newState;

        PlayAudioClipForState(newState);
        UpdateAnimation();

        OnStateChanged?.Invoke(this, newState);
    }

    void PlayAudioClipForState(State state)
    {
        switch (state)
        {
            case State.Idle:
                audioSource.clip = idleAudioClip;
                break;
            case State.Annoyed:
                audioSource.clip = annoyedAudioClip;
                break;
            case State.Angry:
                audioSource.clip = angryAudioClip;
                break;
            case State.Raging:
                audioSource.clip = ragingAudioClip;
                break;
        }

        audioSource.Play();
    }

    void UpdateAnimation()
    {
        animator.SetInteger("State", (int)currentState);

        List<KeyValuePair<AnimationClip, AnimationClip>> overrides =
            new List<KeyValuePair<AnimationClip, AnimationClip>>();

        switch (currentState)
        {
            case State.Idle:
                overrides.Add(
                    new KeyValuePair<AnimationClip, AnimationClip>(
                        GetBaseClipName("idleAnimation"),
                        idleClip
                    )
                );
                break;

            case State.Annoyed:
                overrides.Add(
                    new KeyValuePair<AnimationClip, AnimationClip>(
                        GetBaseClipName("annoyedAnimation"),
                        annoyedClip
                    )
                );
                break;

            case State.Angry:
                overrides.Add(
                    new KeyValuePair<AnimationClip, AnimationClip>(
                        GetBaseClipName("angryAnimation"),
                        angryClip
                    )
                );
                break;

            case State.Raging:
                overrides.Add(
                    new KeyValuePair<AnimationClip, AnimationClip>(
                        GetBaseClipName("ragingAnimation"),
                        ragingClip
                    )
                );
                break;
        }

        overrideController.ApplyOverrides(overrides);
    }

    AnimationClip GetBaseClipName(string clipName)
    {
        foreach (var clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == clipName)
                return clip;
        }
        return null;
    }

    public void SetRage(float newRage)
    {
        rage = Mathf.Clamp(newRage, 0f, maxRage);
        UpdateStateFromRage();
    }

    void UpdateStateFromRage()
    {
        State newState = State.Idle;

        if (rage >= ragingThreshold)
            newState = State.Raging;
        else if (rage >= angryThreshold)
            newState = State.Angry;
        else if (rage >= annoyedThreshold)
            newState = State.Annoyed;

        SetState(newState);
    }

    public void Say(string message)
    {
        GameManager.Instance.ShowSpeech(transform, message);
    }
}
