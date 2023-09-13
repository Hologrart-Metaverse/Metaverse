using UnityEngine;

public class OnLoadUI : MonoBehaviour
{
    void Start()
    {
        PlayerManager.Instance.OnLocalPlayerSpawned += PlayerManager_OnLocalPlayerSpawned;
    }

    private void PlayerManager_OnLocalPlayerSpawned(object sender, System.EventArgs e)
    {
        gameObject.SetActive(false);
    }

}
