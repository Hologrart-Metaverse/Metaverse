using ActionCode.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class GameSO : ScriptableObject
{
    public enum GameId
    {
        nftContest,

    }
    public enum GameType
    {
        _2D,
        _3D,
    }
    public string gameName;
    public GameId gameId;
    public GameType gameType;
    public int maxPlayer;

    [ShowIf("gameType", GameType._2D)]
    public Transform gameCanvas;

    [ShowIf("gameType", GameType._3D)]
    public Transform gameArea;
}
