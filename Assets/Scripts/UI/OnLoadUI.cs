using UnityEngine;

public class OnLoadUI : MonoBehaviour
{
    private void Start()
    {
        Spawner.Instance.OnPlayerSpawned += Spawner_OnPlayerSpawned;
        if (PlayerController.Local != null)
            Destroy(gameObject);
    }

    private void Spawner_OnPlayerSpawned(object sender, System.EventArgs e)
    {
        Destroy(gameObject);
    }
}
