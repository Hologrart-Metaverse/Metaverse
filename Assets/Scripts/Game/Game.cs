using Photon.Pun;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Game : MonoBehaviour
{
    internal bool isOnline;
    private GameSO gameSO;
    internal List<int> players;
    internal int hostId;
    internal GameUI gameUI;
    private GameSO.GameId gameId;
    internal bool isFinished = false;
    internal PhotonView PV;
    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }
    public void InitializeHost(GameSO gameSO, List<int> playerIds, int _hostId)
    {
        InGameMessagesUIHandler.Instance.ChangeChatActivity(false);
        transform.GetChild(0).gameObject.SetActive(true);
        players = playerIds;
        hostId = _hostId;
        gameId = gameSO.gameId;
        Transform playerSpawnPoint = GetComponentInChildren<GameSpawnPoint>().transform;
        TeleportSystem.Instance.TeleportPosition(playerSpawnPoint.position, playerSpawnPoint.rotation);
        this.gameSO = gameSO;
        isOnline = true;
        gameUI = GameCanvasUI.Instance.ShowAndGet(gameSO.gameId);
        gameUI.InitializeVariables(this, true, players, hostId);
        GetReadyToPlay();
        isFinished = false;
    }
    public void InitializeClient(GameSO.GameId _gameId, int[] playerIds, int _hostId)
    {
        InGameMessagesUIHandler.Instance.ChangeChatActivity(false);
        transform.GetChild(0).gameObject.SetActive(true);
        players = playerIds.ToList();
        hostId = _hostId;
        gameId = _gameId;
        isOnline = true;
        Transform playerSpawnPoint = GetComponentInChildren<GameSpawnPoint>().transform;
        TeleportSystem.Instance.TeleportPosition(playerSpawnPoint.position, playerSpawnPoint.rotation);
        gameUI = GameCanvasUI.Instance.ShowAndGet(gameId);
        gameUI.InitializeVariables(this, true, players, hostId);
        isFinished = false;
    }
    public void InitializeOffline(GameSO gameSO)
    {
        InGameMessagesUIHandler.Instance.ChangeChatActivity(false);
        transform.GetChild(0).gameObject.SetActive(true);
        this.gameSO = gameSO;
        gameId = gameSO.gameId;
        isOnline = false;
        Transform playerSpawnPoint = GetComponentInChildren<GameSpawnPoint>().transform;
        TeleportSystem.Instance.TeleportPosition(playerSpawnPoint.position, playerSpawnPoint.rotation);
        gameUI = GameCanvasUI.Instance.ShowAndGet(gameSO.gameId);
        gameUI.InitializeVariables(this, false);
        GetReadyToPlay();
        isFinished = false;
    }
    public void OnFinish()
    {
        if (isFinished)
            return;

        OnFinished();
    }
    public void EndGame()
    {
        InGameMessagesUIHandler.Instance.ChangeChatActivity(true);
        if (isOnline)
        {
            if (PhotonHandler.Instance.IsHostAvailable(hostId))
            {
                GameAreasManager.Instance.MakeAreaEmpty(gameId, GetComponent<PhotonView>().ViewID);
            }
            EndGameOnline();
            OnGameEnded();
        }
        else
        {
            GameAreasManager.Instance.MakeAreaEmpty(gameId, GetComponent<PhotonView>().ViewID);
            EndGameOffline();
            OnGameEnded();
        }
    }
    public abstract void GetReadyToPlay();
    public abstract void Play();
    public abstract void OnFinished();
    public abstract void OnGameEnded();
    public abstract void EndGameOnline();
    public abstract void EndGameOffline();
}
