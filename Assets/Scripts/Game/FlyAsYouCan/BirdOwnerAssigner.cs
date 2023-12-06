using Cinemachine;
using Photon.Pun;
using UnityEngine;

public class BirdOwnerAssigner : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera startCamera;
    [SerializeField] private CinemachineVirtualCamera playerFollowCamera;
    [SerializeField] private Transform stickFollowPoint;
    [SerializeField] private Transform playerFollowPoint;
    [SerializeField] private PhotonView PV;
    public int ownerId = 1;
    public bool isMine;
    public void TakeOwnership()
    {
        isMine = true;
        startCamera.Follow = stickFollowPoint;
        playerFollowCamera.Follow = playerFollowPoint;
        PV.TransferOwnership(PhotonNetwork.LocalPlayer);
    }
    public void ReleaseOwnership()
    {
        isMine = false;
    }
}
