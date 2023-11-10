using Photon.Pun;
using TMPro;
using UnityEngine;

public class UI_EscapeFromMaze : GameUI
{
    [SerializeField] private TextMeshProUGUI clockTMP;
    [SerializeField] private float maxTime;
    private float currentTime;
    [SerializeField] private Countdown countdown;
    [SerializeField] private WinnerDisplay winnerDisplay;
    public override void StartCountdown()
    {
        countdown.StartCountdown(this);
    }
    public override void OnCountdownEnded()
    {
        if (isOnline)
        {
            if (PhotonHandler.Instance.IsHostAvailable(hostId))
            {
                foreach (var plId in memberIds)
                {
                    if (PhotonHandler.Instance.TryGetPlayerByActorNumber(plId, out var player))
                    {
                        PV.RPC(nameof(Initialize), player);
                    }
                }
            }
        }
        else
            Initialize();
    }
    [PunRPC]
    private void Initialize()
    {
        game.Play();
        clockTMP.enabled = true;
        currentTime = maxTime;
        clockTMP.text = maxTime.ToString();
        isInitialized = true;
    }
    private void FixedUpdate()
    {
        if (isInitialized)
        {
            currentTime -= Time.fixedDeltaTime;
            clockTMP.text = Mathf.FloorToInt(currentTime).ToString();

            if (currentTime <= 0)
            {
                clockTMP.text = "0";
                OnFinished("NO ONE :)");
            }
        }
    }

    public override void ResetUI()
    {
        isInitialized = false;
        clockTMP.enabled = false;
    }

    public override void OnFinished(string winnerName)
    {
        isInitialized = false;
        winnerDisplay.ShowWinner(this, winnerName);
    }
}
