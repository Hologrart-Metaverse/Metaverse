using Photon.Pun;
using UnityEngine;

public class GalacticGrid_Obstacle : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.TryGetComponent(out PlayerController player))
        {
            ParticleManager.Instance.Play(ParticleType.Explosion, transform.position, default, .6f);
            if (player.GetComponent<PhotonView>().IsMine)
            {
                player.Teleport(transform.root.GetComponentInChildren<GameSpawnPoint>().transform.position, default, true);
            }

        }
    }
}
