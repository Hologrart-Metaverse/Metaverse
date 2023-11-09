using Photon.Pun;
using System;
using System.Collections;
using System.IO;
using UnityEngine;

public class Spawner : MonoBehaviourPunCallbacks
{
    public static Spawner Instance { get; private set; }
    GameObject controller;
    PhotonView PV;
    public event EventHandler OnPlayerSpawned;
    private void Awake()
    {
        Instance = this;
        PV = GetComponent<PhotonView>();
        if (PlayerController.Local == null)
            StartCoroutine(CreateAfterDelay());
    }
    private IEnumerator CreateAfterDelay()
    {
        yield return new WaitForSeconds(1f);
        CreateController(Utils.GetRandomPositionAtHangar());
    }
    public void PlayerSpawned()
    {
        OnPlayerSpawned?.Invoke(this, EventArgs.Empty);
    }
    void CreateController(Vector3 spawnpoint)
    {
        controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "NetworkPlayer"), spawnpoint, Quaternion.identity, 0, new object[] { PV.ViewID });
    }
    public Vector3 GetRespawnPosition()
    {
        return AreaSystem.Instance.GetCurrentArea().respawnPosition;
    }
}
