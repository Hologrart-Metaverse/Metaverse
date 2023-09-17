using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.XR;
using UnityEngine.EventSystems;

public class InGameMessagesUIHandler : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TextMeshProUGUI chatField;
    [SerializeField] private Button sendMsgButton;
    private bool isChatSelected = false;
    // Start is called before the first frame update
    void Start()
    {
        PlayerManager.Instance.OnLocalPlayerSpawned += PlayerManager_OnLocalPlayerSpawned;
        sendMsgButton.onClick.AddListener(() => SendMessage());
        gameObject.SetActive(false);
    }

    private void PlayerManager_OnLocalPlayerSpawned(object sender, System.EventArgs e)
    {
        gameObject.SetActive(true);
        GameInput.Instance.OnEnterPressed += GameInput_OnEnterPressed;
    }
    private void OnDestroy()
    {
        PlayerManager.Instance.OnLocalPlayerSpawned -= PlayerManager_OnLocalPlayerSpawned;
        GameInput.Instance.OnEnterPressed -= GameInput_OnEnterPressed;
    }
    private void GameInput_OnEnterPressed(object sender, System.EventArgs e)
    {
        if (!isChatSelected)
        {
            if (Utils.IsChooseScreenOn)
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
        string messageWithNickname = NetworkPlayer.Local.NickName.ToString() + ": " + message;
        NetworkInGameMessages.Instance.SendInGameRPCMessage(messageWithNickname);
    }
    public void OnGameMessageReceived(string message)
    {
        chatField.text += message + "\n";
    }
}
