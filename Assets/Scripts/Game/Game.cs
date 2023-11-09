using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Game : MonoBehaviour
{
    internal bool isOnline;
    private GameSO gameSO;
    internal List<int> players;
    private GameUI gameUI;
    public void InitializeHost(GameSO gameSO, List<int> playerIds)
    {
        transform.GetChild(0).gameObject.SetActive(true);
        players = playerIds;
        Transform playerSpawnPoint = GetComponentInChildren<GameSpawnPoint>().transform;
        TeleportSystem.Instance.TeleportPosition(playerSpawnPoint.position, playerSpawnPoint.rotation);
        this.gameSO = gameSO;
        isOnline = true;
        Play();
        gameUI = GameCanvasUI.Instance.ShowAndGet(gameSO.gameId);
        gameUI.Show();
    }
    public void InitializeClient(GameSO.GameId gameId, int[] playerIds)
    {
        players = playerIds.ToList();
        isOnline = true;
        transform.GetChild(0).gameObject.SetActive(true);
        Transform playerSpawnPoint = GetComponentInChildren<GameSpawnPoint>().transform;
        TeleportSystem.Instance.TeleportPosition(playerSpawnPoint.position, playerSpawnPoint.rotation);
        gameUI = GameCanvasUI.Instance.ShowAndGet(gameId);
        gameUI.Show();
    }
    public void InitializeOffline(GameSO gameSO)
    {
        transform.GetChild(0).gameObject.SetActive(true);
        this.gameSO = gameSO;
        isOnline = false;
        Transform playerSpawnPoint = GetComponentInChildren<GameSpawnPoint>().transform;
        TeleportSystem.Instance.TeleportPosition(playerSpawnPoint.position, playerSpawnPoint.rotation);
        Play();
        gameUI = GameCanvasUI.Instance.ShowAndGet(gameSO.gameId);
    }
    public void Finish()
    {
        if (isOnline)
            FinishGameOnline();
        else
            FinishGameOffline();
    }
    public abstract void Play();
    public abstract void FinishGameOnline();
    public abstract void FinishGameOffline();
}
