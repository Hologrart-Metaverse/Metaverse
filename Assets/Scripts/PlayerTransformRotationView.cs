using UnityEngine;
using Photon.Pun;
public class PlayerTransformRotationView : MonoBehaviourPun, IPunObservable
{
    private PhotonView PV;
    [SerializeField] private Transform playerModel;
    [SerializeField] private float lerpSmoothness;
    private Vector3 networkPosition;
    private Quaternion networkRotation;
    private float positionTeleportThreshold = 5f;
    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(playerModel.rotation);
        }
        else if (stream.IsReading)
        {
            networkPosition = (Vector3)stream.ReceiveNext();
            networkRotation = (Quaternion)stream.ReceiveNext();
        }
    }
    private void FixedUpdate()
    {
        if (!PV.IsMine)
        {
            if (Mathf.Abs(networkPosition.magnitude - transform.position.magnitude) > positionTeleportThreshold)
            {
                Debug.Log("threshold büyük tp geldi");
                transform.position = networkPosition;
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, networkPosition, lerpSmoothness * Time.fixedDeltaTime);
            }

            playerModel.rotation = Quaternion.Lerp(playerModel.rotation, networkRotation, lerpSmoothness * Time.fixedDeltaTime);
        }
    }
}
