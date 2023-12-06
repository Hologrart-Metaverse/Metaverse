using Photon.Pun;
using UnityEngine;
public class BallMovementSync : MonoBehaviour, IPunObservable
{
    private Rigidbody _rigidbody;
    private Vector3 _netPosition;

    public bool teleportIfFar;
    public float teleportIfFarDistance;
    [Header("Lerping [Experimental]")]
    public float smoothPos = 5.0f;
    private BirdOwnerAssigner assigner;
    private void Awake()
    {
        PhotonNetwork.SendRate = 80;
        PhotonNetwork.SerializationRate = 80;
        assigner = GetComponentInParent<BirdOwnerAssigner>();
        _rigidbody = gameObject.GetComponent<Rigidbody>();
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(_rigidbody.position);
            stream.SendNext(_rigidbody.velocity);
        }
        else
        {
            _netPosition = (Vector3)stream.ReceiveNext();
            _rigidbody.velocity = (Vector3)stream.ReceiveNext();

            float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
            _netPosition += (_rigidbody.velocity * lag);
        }
    }

    private void FixedUpdate()
    {
        if (assigner.isMine) return;

        _rigidbody.position = Vector3.Lerp(_rigidbody.position, _netPosition, smoothPos * Time.fixedDeltaTime);

        if (Vector3.Distance(_rigidbody.position, _netPosition) > teleportIfFarDistance)
        {
            _rigidbody.position = _netPosition;
        }
    }
}
