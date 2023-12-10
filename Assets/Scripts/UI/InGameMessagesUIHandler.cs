using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Photon.Pun;
using DG.Tweening;
public class InGameMessagesUIHandler : MonoBehaviour
{
    private enum ChatSize
    {
        Bigger, Smaller,
    }
    public static InGameMessagesUIHandler Instance { get; private set; }
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TextMeshProUGUI chatField;
    [SerializeField] private Transform scrollView;
    [SerializeField] private Transform chatTransform;
    //[SerializeField] private Button sendMsgButton;
    private bool isChatSelected = false;
    // Start is called before the first frame update
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        //sendMsgButton.onClick.AddListener(() => SendMessage());
        gameObject.SetActive(false);
        Spawner.Instance.OnPlayerSpawned += Spawner_OnPlayerSpawned;
    }
    public void ChangeChatActivity(bool isActive)
    {
        chatTransform.gameObject.SetActive(isActive);
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
        if (!chatTransform.gameObject.activeSelf)
            return;

        if (!isChatSelected)
        {
            if (Utils.IsUIScreenOpen)
                return;
            ChangeChatSize(ChatSize.Bigger);
            inputField.text = "";
            inputField.Select();
            isChatSelected = true;
        }
        else
        {
            SendMessage();
            ChangeChatSize(ChatSize.Smaller);
        }

    }
    private void SendMessage()
    {
        isChatSelected = false;
        EventSystem.current.SetSelectedGameObject(null);
        string message = inputField.text;

        if (message == "")
            return;

        inputField.text = "";
        string messageWithNickname = PhotonNetwork.LocalPlayer.NickName + ": " + message;
        NetworkInGameMessages.Instance.SendInGameRPCMessage(messageWithNickname);
    }
    private void ChangeChatSize(ChatSize size)
    {
        switch (size)
        {
            case ChatSize.Bigger:
                scrollView.DOScale(1f, .3f).SetEase(Ease.OutBack);
                scrollView.GetComponent<Image>().DOFade(1f, .3f);
                inputField.transform.DOScale(1f, .3f).SetEase(Ease.OutBack);
                inputField.GetComponent<Image>().DOFade(1f, .3f);
                break;
            case ChatSize.Smaller:
                scrollView.DOScale(.9f, .3f).SetEase(Ease.InBack);
                scrollView.GetComponent<Image>().DOFade(.5f, .3f);
                inputField.transform.DOScale(.9f, .3f).SetEase(Ease.InBack);
                inputField.GetComponent<Image>().DOFade(.5f, .3f);
                break;
        }
    }
    public void OnGameMessageReceived(string message)
    {
        chatField.text += message + "\n";
    }
}
