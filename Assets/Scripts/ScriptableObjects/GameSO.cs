using UnityEngine;

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
    public Transform gameArea;

    public Vector3 gameAreaSpawnPoint;
}
