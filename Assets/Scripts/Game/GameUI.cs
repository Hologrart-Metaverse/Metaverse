using Photon.Pun;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class GameUI : MonoBehaviour
{
    public TextMeshProUGUI clockTMP;
    public float maxTime;
    public Countdown countdown;
    public WinnerDisplay winnerDisplay;
    internal List<int> memberIds = new();
    internal int hostId;
    internal bool isOnline;
    internal bool isInitialized = false;
    internal PhotonView PV;
    internal Game game;
    internal float currentTime;
    public void InitializeVariables(Game _game, bool _isOnline, List<int> _memberIds = default, int _hostId = default)
    {
        PV = GetComponent<PhotonView>();
        isOnline = _isOnline;
        memberIds = _memberIds;
        hostId = _hostId;
        game = _game;
    }
    public void OnGameEnded()
    {
        game.EndGame();
        ResetUI();
    }
    public abstract void StartCountdown();
    public abstract void OnCountdownEnded();
    public abstract void OnFinished(string winner);
    public abstract void ResetUI();
    public void Show()
    {
        isInitialized = true;
    }
}
