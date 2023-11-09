using Photon.Pun;
using UnityEngine;

public class Game_EscapeFromMaze : Game
{
    private PhotonView PV;
    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }
    public override void Play()
    {
        if (isOnline)
        {
            foreach (int playerId in players)
            {
                var pl = PhotonHandler.Instance.GetPlayerByActorNumber(playerId);
                PV.RPC(nameof(InitializeRPC), pl);
            }
        }
        else
        {

        }

    }

    public override void FinishGameOnline()
    {
        foreach (int playerId in players)
        {
            var pl = PhotonHandler.Instance.GetPlayerByActorNumber(playerId);
            if (pl != null)
                PV.RPC(nameof(FinishGameOnlineRPC), pl);
        }
    }
    [PunRPC]
    private void FinishGameOnlineRPC()
    {
        GameCanvasUI.Instance.Hide();
        TeleportSystem.Instance.TeleportArea(Area.Game_Planet);
    }
    public override void FinishGameOffline()
    {
        GameCanvasUI.Instance.Hide();
        TeleportSystem.Instance.TeleportArea(Area.Game_Planet);
    }
    [PunRPC]
    private void InitializeRPC()
    {
        Debug.Log("BU MESAJ HERKESDE VARSA TAMAMDIR...");
    }


}
