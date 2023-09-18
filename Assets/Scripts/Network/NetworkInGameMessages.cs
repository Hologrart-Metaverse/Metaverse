using UnityEngine;
using Photon.Pun;

public class NetworkInGameMessages : MonoBehaviourPunCallbacks
{
    public static NetworkInGameMessages Instance;

    [SerializeField] InGameMessagesUIHandler inGameMessagesUIHander;

    private PhotonView PV;
    private void Awake()
    {
        Instance = this;
        PV = GetComponent<PhotonView>();
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
        PV.RPC(nameof(RPC_InGameMessage), RpcTarget.All, message);
    }
    [PunRPC]
    void RPC_InGameMessage(string message)
    {
        inGameMessagesUIHander.OnGameMessageReceived(message);
    }
}
