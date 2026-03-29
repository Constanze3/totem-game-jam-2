using UnityEngine;
using System.Collections.Generic;

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

    public float rage = 0f;
    public float rage_rate = 1f;
    public float max_rage = 100f;

    private Animator animator;
    private AnimatorOverrideController overrideController;

    [Header("Animation Clips")]
    [SerializeField]
    private AnimationClip idleClip;

    [SerializeField]
    private AnimationClip annoyedClip;

    [SerializeField]
    private AnimationClip angryClip;

    [SerializeField]
    private AnimationClip ragingClip;

    void Start()
    {
        animator = GetComponent<Animator>();

        // Create override controller from the base controller
        overrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);

        animator.runtimeAnimatorController = overrideController;

        UpdateAnimation();
    }

    void Update()
    {
        IncreaseRageOverTime();
    }

    void IncreaseRageOverTime()
    {
        SetRage(rage + rage_rate * Time.deltaTime);
    }

    public void SetState(State newState)
    {
        if (currentState == newState)
            return;

        OnStateChanged?.Invoke(this, newState);

        currentState = newState;
        UpdateAnimation();
    }

    void UpdateAnimation()
    {
        // Update animator parameter
        animator.SetInteger("State", (int)currentState);

        // Swap animation clips based on state
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

    // Helper to map base clip name
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
        rage = Mathf.Clamp(newRage, 0f, max_rage);
        UpdateStateFromRage();
    }

    void UpdateStateFromRage()
    {
        if (rage > 75)
            SetState(State.Raging);
        else if (rage > 50)
            SetState(State.Angry);
        else if (rage > 25)
            SetState(State.Annoyed);
        else
            SetState(State.Idle);
    }
}
