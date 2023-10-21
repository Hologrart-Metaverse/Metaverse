using UnityEngine;

public enum State
{
    None,
    ArtPlanet_StaticUI,
    ArtPlanet_DynamicUI,
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
    public bool IsMovable()
    {
        return currentState == State.None;
    }
    public void SetState(State newState)
    {
        currentState = newState;
        StateChanged?.Invoke(this, currentState);
    }
    public State GetState()
    {
        return currentState;
    }
}
