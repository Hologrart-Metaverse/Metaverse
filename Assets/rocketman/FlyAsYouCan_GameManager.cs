using System;
using UnityEngine;

public class FlyAsYouCan_GameManager : MonoBehaviour
{
    public static FlyAsYouCan_GameManager Instance { get; private set; }
    public enum State
    {
        Ready,
        Playing,
        TryAgain,
        GameOver,
    }
    private State state;
    public event EventHandler<State> OnStateChanged;

    private void OnEnable()
    {
        Instance = this;
        ChangeState(State.Ready);
    }
    // Güncel state i güncelleyen fonksiyon
    public void ChangeState(State _state)
    {
        state = _state;
        OnStateChanged?.Invoke(this, state);
    }
    public State GetState()
    {
        return state;
    }
}
