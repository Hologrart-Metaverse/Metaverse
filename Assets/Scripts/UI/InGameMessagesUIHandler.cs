using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Photon.Pun;

public class InGameMessagesUIHandler : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TextMeshProUGUI chatField;
    [SerializeField] private Button sendMsgButton;
    private bool isChatSelected = false;
    // Start is called before the first frame update
    void Start()
    {
        sendMsgButton.onClick.AddListener(() => SendMessage());
        gameObject.SetActive(false);
        Spawner.Instance.OnPlayerSpawned += Spawner_OnPlayerSpawned;
    }

    private void Spawner_OnPlayerSpawned(object sender, System.EventArgs e)
    {
        gameObject.SetActive(true);
        GameInput.Instance.OnEnterPressed += GameInput_OnEnterPressed;
    }


    private void OnDestroy()
    {
        Spawner.Instance.OnPlayerSpawned -= Spawner_OnPlayerSpawned;
        GameInput.Instance.OnEnterPressed -= GameInput_OnEnterPressed;
    }
    private void GameInput_OnEnterPressed(object sender, System.EventArgs e)
    {
        if (!isChatSelected)
        {
            if (Utils.IsUIScreenOpen)
                return;

            inputField.text = "";
            Utils.SetMouseLockedState(false);
            inputField.Select();
            isChatSelected = true;
        }
        else
        {
            SendMessage();
        }

    }
    private void SendMessage()
    {
        Utils.SetMouseLockedState(true);
        isChatSelected = false;
        EventSystem.current.SetSelectedGameObject(null);
        string message = inputField.text;

        if (message == "")
            return;

        inputField.text = "";
        string messageWithNickname = PhotonNetwork.LocalPlayer.NickName + ": " + message;
        NetworkInGameMessages.Instance.SendInGameRPCMessage(messageWithNickname);
    }
    public void OnGameMessageReceived(string message)
    {
        chatField.text += message + "\n";
    }
}
