using Photon.Pun;
using TMPro;
using UnityEngine;

public class UI_EscapeFromMaze : GameUI
{
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
