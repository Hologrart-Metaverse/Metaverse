using Fusion;
using System;
using UnityEngine;

public class PlayerManager : NetworkBehaviour
{
    public static PlayerManager Instance;

    [Networked(OnChanged = nameof(OnPlayerListChanged)), Capacity(100)] //Capacity is gonna be the max player size later on
    public NetworkLinkedList<NetworkPlayer> _players => default;
    public event EventHandler OnLocalPlayerSpawned;

    private void Awake()
    {
        Instance = this;
    }
    static void OnPlayerListChanged(Changed<PlayerManager> changed)
    {
        //all the players will know that when player list has changed
    }
    public void OnSpawned()
    {
        OnLocalPlayerSpawned?.Invoke(this, EventArgs.Empty);
    }
    public void AddPlayer(NetworkPlayer player)
    {
        if (Object.HasStateAuthority)
        {
            _players.Add(player);
        }
    }
    public void RemovePlayer(NetworkPlayer player)
    {
        if (Object.HasStateAuthority)
        {
            _players.Remove(player);
        }
    }
    public NetworkString<_16> GetPlayerNickname(NetworkId id)
    {
        NetworkPlayer player = GetPlayerById(id);
        return player.NickName;
    }
    private NetworkPlayer GetPlayerById(NetworkId id)
    {
        foreach (NetworkPlayer player in _players)
        {
            if (id == player.Object.Id) return player;
        }
        return null;
    }
}
