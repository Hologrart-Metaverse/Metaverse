using UnityEngine;

public class OnLoadUI : MonoBehaviour
{
    private void Start()
    {
        Spawner.Instance.OnPlayerSpawned += Spawner_OnPlayerSpawned;
    }

    private void Spawner_OnPlayerSpawned(object sender, System.EventArgs e)
    {
        Destroy(gameObject);
    }
}
