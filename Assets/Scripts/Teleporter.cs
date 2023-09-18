using Photon.Pun;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.TryGetComponent(out PhotonView PV))
        {
            if (PV.IsMine)
                TeleportUI.Instance.Show();

        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.transform.TryGetComponent(out PhotonView PV))
        {
            if (PV.IsMine)
                TeleportUI.Instance.Hide();
        }
    }

}
