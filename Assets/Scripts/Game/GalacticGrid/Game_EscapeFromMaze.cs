using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class Game_EscapeFromMaze : Game
{
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
    public override void Play()
    {
        GetComponentInChildren<Obstacle>().GetComponent<Collider>().enabled = false;
    }

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
    [PunRPC]
    private void StartCountdown()
    {
        UI_EscapeFromMaze uiEscapeFromMaze = gameUI as UI_EscapeFromMaze;
        uiEscapeFromMaze.StartCountdown();
    }
    public override void OnLeavingFromGame()
    {
        GetComponentInChildren<Obstacle>().GetComponent<Collider>().enabled = true;
        transform.GetChild(0).gameObject.SetActive(false);
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
}
