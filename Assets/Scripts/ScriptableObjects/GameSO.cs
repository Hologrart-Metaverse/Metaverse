using Newtonsoft.Json;
using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu()]
public class GameSO : ScriptableObject
{
    public enum GameId
    {
        nftContest,
        escapeFromMaze,
        flyAsYouCan,
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
    [JsonIgnore]
    public Transform gameArea;
    public bool isChatVisibleOnPlay;
    public Vector3 gameAreaSpawnPoint;
}
