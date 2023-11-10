using System;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class GameCanvasUIArgs
{
    public GameSO.GameId gameId;
    public Transform gameUITransform;
}
public class GameCanvasUI : MonoBehaviour
{
    public static GameCanvasUI Instance { get; private set; }
    [SerializeField] private List<GameCanvasUIArgs> gamesUI = new List<GameCanvasUIArgs>();
    private Transform lastGameUITransform;
    private void Awake()
    {
        Instance = this;
    }

    public GameUI ShowAndGet(GameSO.GameId gameId)
    {
        foreach (var gameUI in gamesUI)
        {
            if (gameUI.gameId == gameId)
            {
                Debug.Log(gameUI.gameUITransform.name + " dönülüyoorr");
                lastGameUITransform = gameUI.gameUITransform;
                lastGameUITransform.GetChild(0).gameObject.SetActive(true);
                return lastGameUITransform.GetComponent<GameUI>();
            }
        }
        return null;
    }
    public void Hide()
    {
        lastGameUITransform.GetChild(0).gameObject.SetActive(false);
    }
}
