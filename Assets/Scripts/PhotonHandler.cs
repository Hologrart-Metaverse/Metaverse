using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;

public class PhotonHandler : MonoBehaviourPunCallbacks
{
    public static PhotonHandler Instance { get; private set; }
    private Dictionary<int, Player> players = new();
    private void Awake()
    {
        Instance = this;
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        players.Add(newPlayer.ActorNumber, newPlayer);
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        players.Remove(otherPlayer.ActorNumber);
    }
    void Start()
    {
        Spawner.Instance.OnPlayerSpawned += Spawner_OnPlayerSpawned;
    }

    private void Spawner_OnPlayerSpawned(object sender, System.EventArgs e)
    {
        foreach (var pl in PhotonNetwork.PlayerList)
        {
            players.Add(pl.ActorNumber, pl);
        }
    }
    public bool TryGetPlayerByActorNumber(int actorNumber, out Player pl)
    {
        pl = default;
        if (players.ContainsKey(actorNumber))
        {
            pl = players[actorNumber];
            return true;
        }
        return false;
    }
    public Player GetPlayerByActorNumber(int actorNumber)
    {
        if (players.ContainsKey(actorNumber))
        {
            return players[actorNumber];
        }
        return null;
    }
    public bool IsHostAvailable(int hostId)
    {
        return GetPlayerByActorNumber(hostId) == null || hostId == PhotonNetwork.LocalPlayer.ActorNumber;
    }
}
