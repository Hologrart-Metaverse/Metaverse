using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
public class GameSystem : MonoBehaviourPun
{
    public static GameSystem Instance { get; private set; }
    private PhotonView PV;
    public bool InGame { get; set; }
    private List<int> currentRoomMemberIds;
    private GameSO currentGameSO;
    private void Awake()
    {
        Instance = this;
        PV = GetComponent<PhotonView>();
    }
    public void StartOnlineGame(GameSO gameSO, GameRoom gameRoom)
    {
        currentRoomMemberIds = gameRoom.roomMembersIds;
        currentGameSO = gameSO;

        //Check if is there any empty area
        int viewId = GameAreasManager.Instance.FindEmptyAreaViewId(gameSO.gameId);
        if (viewId == -1)
        {
            //There is no suit game area so we want from master client to create a new one
            PV.RPC(nameof(CreateGameAreaMasterClient), PhotonNetwork.MasterClient,
               gameSO.gameId, gameSO.gameName, gameSO.gameAreaSpawnPoint, PhotonNetwork.LocalPlayer.ActorNumber, true);
        }
        else
        {
            EnterGameHost(viewId);
        }
    }
    public void StartOfflineGame(GameSO gameSO)
    {
        currentGameSO = gameSO;
        //Check if is there any empty area
        int viewId = GameAreasManager.Instance.FindEmptyAreaViewId(gameSO.gameId);
        if (viewId == -1)
        {
            //There is no suit game area so we want from master client to create a new one
            PV.RPC(nameof(CreateGameAreaMasterClient), PhotonNetwork.MasterClient,
               gameSO.gameId, gameSO.gameName, gameSO.gameAreaSpawnPoint, PhotonNetwork.LocalPlayer.ActorNumber, false);
        }
        else
        {
            EnterGameOffline(gameSO.gameId, viewId);
        }
    }
    [PunRPC]
    private void CreateGameAreaMasterClient(GameSO.GameId gameId, string gameName, Vector3 areaPosition, int hostId, bool isOnline)
    {
        int areaCount = GameAreasManager.Instance.GetAreaCountByGameId(gameId);
        GameObject gameAreaObj = PhotonNetwork.InstantiateRoomObject(Path.Combine("PhotonPrefabs", "Games", gameName),
            areaPosition + new Vector3(0, areaCount * 100, 0), Quaternion.identity, 0, new object[] { PV.ViewID });
        if (isOnline)
            PV.RPC(nameof(EnterGameHost), PhotonHandler.Instance.GetPlayerByActorNumber(hostId), gameAreaObj.GetComponent<PhotonView>().ViewID);
        else
            PV.RPC(nameof(EnterGameOffline), PhotonHandler.Instance.GetPlayerByActorNumber(hostId), gameId, gameAreaObj.GetComponent<PhotonView>().ViewID);
    }

    [PunRPC]
    private void EnterGameHost(int areaViewId)
    {
        GameAreasManager.Instance.AddOrFillGameArea(currentGameSO.gameId, areaViewId);
        foreach (var memberId in currentRoomMemberIds)
        {
            if (memberId == PhotonNetwork.LocalPlayer.ActorNumber)
                continue;
            if (PhotonHandler.Instance.TryGetPlayerByActorNumber(memberId, out Player pl))
            {
                PV.RPC(nameof(EnterGameClient), pl, currentGameSO.gameId, areaViewId, currentRoomMemberIds.ToArray(), PhotonNetwork.LocalPlayer.ActorNumber);
            }
        }
        PhotonNetwork.GetPhotonView(areaViewId).GetComponent<Game>().InitializeHost(currentGameSO, currentRoomMemberIds, PhotonNetwork.LocalPlayer.ActorNumber);
    }
    [PunRPC]
    private void EnterGameClient(GameSO.GameId gameId, int areaViewId, int[] playerIds, int hostId)
    {
        PhotonNetwork.GetPhotonView(areaViewId).GetComponent<Game>().InitializeClient(gameId, playerIds, hostId);
    }
    [PunRPC]
    private void EnterGameOffline(GameSO.GameId gameId, int areaViewId)
    {
        GameAreasManager.Instance.AddOrFillGameArea(gameId, areaViewId);
        PhotonNetwork.GetPhotonView(areaViewId).GetComponent<Game>().InitializeOffline(currentGameSO);
    }
}
