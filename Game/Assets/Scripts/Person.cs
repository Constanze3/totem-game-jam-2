using System;
using System.Collections.Generic;
using UnityEngine;

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
        CardSwipe,
        CloseAds,
        Frogger,
    };

    public string personName;

    [SerializeField]
    public State currentState = State.Idle;

    public bool satisfied = false;

    public float rage = 0f;

    public float rage_rate = 1f; // How many rage gained per second

    public float max_rage = 100f;

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        UpdateAnimation();
    }

    void Update()
    {
        if (!satisfied)
        {
            SetRage(rage + rage_rate * Time.deltaTime);
        }
    }

    public void SetState(State newState)
    {
        currentState = newState;
        UpdateAnimation();
    }

    void UpdateAnimation()
    {
        animator.SetInteger("State", (int)currentState);
    }

    public void SetRage(float newRage)
    {
        rage = newRage;

        rage = Mathf.Clamp(rage, 0f, max_rage);

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
