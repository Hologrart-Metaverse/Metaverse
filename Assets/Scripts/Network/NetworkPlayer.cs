using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour, IPlayerLeft
{
    public static NetworkPlayer Local { get; set; }
    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            Local = this;

            Debug.Log("Local player spawned..");

            //Disable main camera

            Camera.main.gameObject.SetActive(false);
        }
        else
        {
            Camera localCamera = GetComponentInChildren<Camera>();
            Destroy(localCamera);
            Debug.Log("Remote player spawned..");
        }
    }
    public void PlayerLeft(PlayerRef player)
    {
        if (player == Object.InputAuthority)
        {
            Runner.Despawn(Object);
        }
    }


}
