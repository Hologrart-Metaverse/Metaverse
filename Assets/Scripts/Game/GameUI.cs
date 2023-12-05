using Photon.Pun;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class GameUI : MonoBehaviour
{
    internal TextMeshProUGUI clockTMP;
    public float maxTime;
    internal Countdown countdown;
    internal WinnerDisplay winnerDisplay;
    internal List<int> memberIds = new();
    internal int hostId;
    internal bool isOnline;
    internal bool isInitialized = false;
    internal PhotonView PV;
    internal Game game;
    internal float currentTime;
    private string second;
    private void Start()
    {
        winnerDisplay = GetComponentInChildren<WinnerDisplay>(true);
        countdown = GetComponentInChildren<Countdown>(true);
        clockTMP = GetComponentInChildren<GameClock>(true).GetComponent<TextMeshProUGUI>();
    }
    public void InitializeVariables(Game _game, bool _isOnline, List<int> _memberIds = default, int _hostId = default)
    {
        PV = GetComponent<PhotonView>();
        isOnline = _isOnline;
        memberIds = _memberIds;
        hostId = _hostId;
        game = _game;
    }
    private void FixedUpdate()
    {
        if (isInitialized)
        {
            currentTime -= Time.fixedDeltaTime;
            second = currentTime % 60 < 10 ? "0" + Mathf.FloorToInt(currentTime % 60) : Mathf.FloorToInt(currentTime % 60).ToString();
            if (currentTime > 59)
            {
                clockTMP.text = Mathf.FloorToInt(currentTime / 60) + ":" + second;
            }
            else
                clockTMP.text = "0:" + second;

            if (currentTime <= 0)
            {
                clockTMP.text = "TIME IS OVER";
                OnFinished("NO ONE :)");
            }
        }
    }
    public void OnWinnerShowed()
    {
        game.EndGame();
        ResetUI();
    }
    public abstract void StartCountdown();
    public abstract void OnCountdownEnded();
    public abstract void OnFinished(string winner);
    public abstract void ResetUI();
}
