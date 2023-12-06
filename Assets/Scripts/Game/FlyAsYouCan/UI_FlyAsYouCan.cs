using Photon.Pun;
using UnityEngine;

public class UI_FlyAsYouCan : GameUI
{
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
    public override void OnFinished(string winner)
    {
        isInitialized = false;
        winnerDisplay.ShowWinner(this, winner);
    }

    public override void ResetUI()
    {
        isInitialized = false;
        clockTMP.enabled = false;
    }

    public override void StartCountdown()
    {
        countdown.StartCountdown(this);

    }
}
