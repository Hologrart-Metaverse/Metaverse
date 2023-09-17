using UnityEngine;

public class Teleporter : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.TryGetComponent(out NetworkPlayer player))
        {
            TeleportUI.Instance.Show();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.transform.TryGetComponent(out NetworkPlayer player))
        {
            TeleportUI.Instance.Hide();
        }
    }
}
