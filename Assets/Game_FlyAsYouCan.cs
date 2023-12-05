using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using UnityEngine;

public class Game_FlyAsYouCan : Game
{
    [SerializeField] private RewardCreator _rewardCreator;
    [SerializeField] private Transform playersTransform;
    [SerializeField] private FlyAsYouCan_GameManager gameManager;
    private BirdOwnerAssigner _ownerAssigner;
    public override void EndGameOffline()
    {
        GameCanvasUI.Instance.Hide();
        TeleportSystem.Instance.TeleportArea(Area.Game_Planet);
    }
    public override void EndGameOnline()
    {
        if (isOnline)
        {
            GameCanvasUI.Instance.Hide();
            TeleportSystem.Instance.TeleportArea(Area.Game_Planet);
        }
    }
    public override void GetReadyToPlay()
    {
        _rewardCreator.Produce();
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
        if (_ownerAssigner != null)
            _ownerAssigner.ReleaseOwnership();

        UI_FlyAsYouCan uiFlyAsYouCan = gameUI as UI_FlyAsYouCan;
        uiFlyAsYouCan.StartCountdown();
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
        gameManager.ChangeState(FlyAsYouCan_GameManager.State.GameOver);
        isFinished = true;
        gameUI.OnFinished(winnerName);
    }
    public override void OnLeavingFromGame()
    {
        Utils.SetMouseLockedState(true);
        transform.GetChild(0).gameObject.SetActive(false);
    }
    public override void Play()
    {
        Utils.SetMouseLockedState(false);
        if (isOnline)
        {
            _ownerAssigner = playersTransform.GetComponentsInChildren<BirdOwnerAssigner>()
         .Where(x => x.ownerId == players.IndexOf(PhotonNetwork.LocalPlayer.ActorNumber)).FirstOrDefault();
            _ownerAssigner.TakeOwnership();
        }
        else
        {
            _ownerAssigner = playersTransform.GetChild(0).GetComponent<BirdOwnerAssigner>();
            _ownerAssigner.TakeOwnership();
        }
    }
}
