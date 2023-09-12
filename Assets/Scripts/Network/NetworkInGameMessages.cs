using UnityEngine;
using Fusion;

public class NetworkInGameMessages : NetworkBehaviour
{
    public static NetworkInGameMessages Instance;

    [SerializeField] InGameMessagesUIHandler inGameMessagesUIHander;
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        if (inGameMessagesUIHander == null)
        {
            inGameMessagesUIHander = FindObjectOfType<InGameMessagesUIHandler>();
        }
    }

    public void SendInGameRPCMessage(string message)
    {
        RPC_InGameMessage(message);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    void RPC_InGameMessage(string message, RpcInfo info = default)
    {
        Debug.Log($"[RPC] InGameMessage {message}");
        inGameMessagesUIHander.OnGameMessageReceived(message);
    }
}
