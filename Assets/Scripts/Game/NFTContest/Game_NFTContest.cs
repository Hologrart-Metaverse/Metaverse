using Photon.Pun;
using Photon.Realtime;

public class Game_NFTContest : Game
{
    public override void EndGameOnline()
    {
        if (isOnline)
        {
            GameCanvasUI.Instance.Hide();
            TeleportSystem.Instance.TeleportArea(Area.Game_Planet);
        }
    }
    public override void EndGameOffline()
    {
        GameCanvasUI.Instance.Hide();
        TeleportSystem.Instance.TeleportArea(Area.Game_Planet);
    }
    public override void GetReadyToPlay()
    {
        if (isOnline)
        {
            foreach (int playerId in players)
            {
                if (PhotonHandler.Instance.TryGetPlayerByActorNumber(playerId, out Player pl))
                {
                    PV.RPC(nameof(StartCountdown), pl);
                }
            }
        }
        else
        {
            StartCountdown();
        }
    }
    [PunRPC]
    private void StartCountdown()
    {
        UI_NFTContest uiNFTContest = gameUI as UI_NFTContest;
        uiNFTContest.StartCountdown();
    }
    public override void OnFinished()
    {
        if (isOnline)
        {
            foreach (int playerId in players)
            {
                if (PhotonHandler.Instance.TryGetPlayerByActorNumber(playerId, out Player pl))
                {
                    PV.RPC(nameof(OnFinishedRpc), pl, PhotonNetwork.LocalPlayer.NickName);
                }
            }
        }
        else
            OnFinishedRpc(PhotonNetwork.LocalPlayer.NickName);
    }
    [PunRPC]
    private void OnFinishedRpc(string winnerName)
    {
        isFinished = true;
        gameUI.OnFinished(winnerName);
    }
    public override void OnLeavingFromGame()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }

    public override void Play()
    {
    }
}
