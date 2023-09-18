using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class Launcher : MonoBehaviourPunCallbacks
{
    public static Launcher Instance;
    [SerializeField] TMP_Text errorText;
    [SerializeField] GameObject sameNicknameAlert;
    public const string PLAYER_NICKNAME = "PlayerNickname";
    public TMP_InputField inputField;
    [SerializeField] private TextMeshProUGUI loadingText;
    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        MenuManager.Instance.OpenMenu("loading");

        if (PlayerPrefs.HasKey(PLAYER_NICKNAME))
            inputField.text = PlayerPrefs.GetString(PLAYER_NICKNAME);
        else
            inputField.text = "Player" + Random.Range(100, 999);

        //PlayerProperties.Instance?.ClearAllProperties();
        Debug.Log("Connecting to Master");
        PhotonNetwork.ConnectUsingSettings();
    }
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master");
        if (!PhotonNetwork.InLobby)
            PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnJoinedLobby()
    {
        MenuManager.Instance.OpenMenu("title");
    }
    public override void OnJoinedRoom()
    {
        bool isRoomAvailable = true;
        foreach (Player pl in PhotonNetwork.PlayerListOthers)
        {
            if (pl.NickName.ToLower() == PhotonNetwork.LocalPlayer.NickName.ToLower())
            {
                isRoomAvailable = false;
                break;
            }
        }
        if (!isRoomAvailable)
        {
            PhotonNetwork.LeaveRoom();
            sameNicknameAlert.SetActive(true);
            return;
        }
        if (SceneManager.GetActiveScene().name == "MainMenuScene")
            SceneManager.LoadScene("GameScene");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        errorText.text = "Room Creation Failed: " + message;
        Debug.LogError("Room Creation Failed: " + message);
        MenuManager.Instance.OpenMenu("error");
    }
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        MenuManager.Instance.OpenMenu("loading");
    }
    public void OnClick_CloseSameNicknameAlert()
    {
        sameNicknameAlert.SetActive(false);
    }
    public void JoinOrCreateRoom()
    {
        PlayerPrefs.SetString(PLAYER_NICKNAME, inputField.text);
        PlayerPrefs.Save();
        PhotonNetwork.LocalPlayer.NickName = inputField.text;
        PhotonNetwork.JoinRandomOrCreateRoom();
        MenuManager.Instance.OpenMenu("loading");
    }
    public override void OnLeftRoom()
    {
        if (SceneManager.GetActiveScene().name == "GameScene")
            SceneManager.LoadScene("MainMenuScene");
        MenuManager.Instance.OpenMenu("title");
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        errorText.text = "Joined room Failed: " + message;
        MenuManager.Instance.OpenMenu("error");
    }
}