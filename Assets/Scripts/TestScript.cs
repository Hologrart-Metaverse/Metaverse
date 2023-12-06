using Photon.Pun;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = false;
        Utils.SetMouseLockedState(false);
    }
    public void ChangeSceneToTest()
    {
        PhotonNetwork.LoadLevel(2);
    }
    public void ChangeSceneToBack()
    {
        PhotonNetwork.LoadLevel(1);
    }
}
