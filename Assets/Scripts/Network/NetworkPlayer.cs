using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using TMPro;

public class NetworkPlayer : NetworkBehaviour, IPlayerLeft
{
    public TextMeshProUGUI playerNickNameTM;
    public static NetworkPlayer Local { get; set; }
    //public Transform playerModel;

    [Networked(OnChanged = nameof(OnNickNameChanged))]
    [HideInInspector] public NetworkString<_16> NickName { get; set; }
    [HideInInspector] public int PlayerId { get; set; }

    //Other components
    NetworkInGameMessages networkInGameMessages;

    void Awake()
    {
        networkInGameMessages = GetComponent<NetworkInGameMessages>();
    }

    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            Local = this;

            //Sets the layer of the local players model
            //Utils.SetRenderLayerInChildren(playerModel, LayerMask.NameToLayer("LocalPlayerModel"));

            //Disable main camera
            Camera.main.gameObject.SetActive(false);

            RPC_SetNickName(PlayerPrefs.GetString(MainMenuUIHandler.PLAYER_NICKNAME));
            PlayerManager.Instance.OnSpawned();
            Debug.Log("Spawned local player");
        }
        else
        {
            //Disable the camera if we are not the local player
            Camera localCamera = GetComponentInChildren<Camera>();
            localCamera.enabled = false;

            //Only 1 audio listner is allowed in the scene so disable remote players audio listner
            AudioListener audioListener = GetComponentInChildren<AudioListener>();
            audioListener.enabled = false;

            //Disable UI for remote player
            //localUI.SetActive(false);

            Debug.Log("Spawned remote player");
        }

        //Set the player as a player object
        Runner.SetPlayerObject(Object.InputAuthority, Object);

        //Make it easier to tell which player is which.
        transform.name = $"P_{Object.Id}";
    }

    public void PlayerLeft(PlayerRef player)
    {
        if (player == Object.InputAuthority)
            Runner.Despawn(Object);
    }
    static void OnNickNameChanged(Changed<NetworkPlayer> changed)
    {
        changed.Behaviour.OnNickNameChanged();
    }

    private void OnNickNameChanged()
    {
        playerNickNameTM.text = NickName.ToString();
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_SetNickName(string nickName, RpcInfo info = default)
    {
        this.NickName = nickName;
    }
    public void TeleportPlayer(Vector3 pos)
    {
        GetComponent<NetworkCharacterControllerPrototypeCustom>().Teleport(pos);
    }
 
}
