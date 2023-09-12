using Fusion;
using System;
using UnityEngine;

public class PlayerManager : NetworkBehaviour
{
    public static PlayerManager Instance;

    [Networked, Capacity(100)] //Capacity is gonna be the max player size later on
    public NetworkLinkedList<NetworkPlayer> _players => default;
    public event EventHandler OnLocalPlayerSpawned;
    private void Awake()
    {
        Instance = this;
    }
    public void OnSpawned()
    {
        OnLocalPlayerSpawned?.Invoke(this, EventArgs.Empty);
    }
    public void AddPlayer(NetworkPlayer player)
    {
        RPC_AddPlayer(player);
    }
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_AddPlayer(NetworkPlayer player, RpcInfo info = default)
    {
        _players.Add(player);
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
