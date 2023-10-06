﻿using UnityEngine;

public enum State
{
    None,
    EditingNFTScreen,
}
public class StateHandler : MonoBehaviour
{
    public static StateHandler Instance;

    private State currentState = State.None;

    public delegate void OnStateChanged(StateHandler sender, State state);
    public OnStateChanged StateChanged;
    private void Awake()
    {
        Instance = this;
    }
   
    public void SetState(State newState)
    {
        currentState = newState;
        StateChanged?.Invoke(this, currentState);
    }
}
