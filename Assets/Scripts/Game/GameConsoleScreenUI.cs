using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class GameConsoleScreenUI : MonoBehaviour
{
    #region Variables
    [SerializeField] private Transform firstMenuContainer;
    [SerializeField] private Transform onePlayerMenuContainer;
    [SerializeField] private Transform multiplayerMenuContainer;
    [SerializeField] private Transform multiplayerCreateOrJoinMenu;
    [SerializeField] private Transform multiplayerRoomMenu;
    [SerializeField] private Transform multiplayerRoomListMenu;
    [SerializeField] private Button onePlayerEnterBtn;
    [SerializeField] private Button onePlayerBackBtn;
    [SerializeField] private Button onePlayerStartGameBtn;
    [SerializeField] private Button multiplayerEnterBtn;
    [SerializeField] private Button multiplayerBackBtn;
    [SerializeField] private Button multiplayerCreateRoomBtn;
    [SerializeField] private Button multiplayerJoinGameBtn;
    [SerializeField] private Button multiplayerStartGameBtn;
    [SerializeField] private TextMeshProUGUI gameNameTMP;
    [SerializeField] private TextMeshProUGUI multiplayerRoomInfoText;
    [SerializeField] private TextMeshProUGUI waitingHostToStartTMP;
    [SerializeField] private Transform roomListButtonPrefab;
    [SerializeField] private Transform roomListButtonsContainer;
    [SerializeField] private Transform roomMemberPrefab;
    [SerializeField] private Transform roomMembersContainer;
    private GameConsole gameConsole;
    private List<Transform> roomListButtons = new();
    private List<Transform> roomMembers = new();
    private GameSO currentGameSO;
    private Player gameHost;
    #endregion
    private void Awake()
    {
        gameConsole = GetComponentInParent<GameConsole>();
        roomListButtonPrefab.gameObject.SetActive(false);
        roomMemberPrefab.gameObject.SetActive(false);
    }
    private void Start()
    {
        onePlayerEnterBtn.onClick.AddListener(() => HideAndShow(firstMenuContainer, onePlayerMenuContainer));
        onePlayerBackBtn.onClick.AddListener(() => HideAndShow(onePlayerMenuContainer, firstMenuContainer));
        onePlayerStartGameBtn.onClick.AddListener(() => gameConsole.StartOfflineGame());

        multiplayerEnterBtn.onClick.AddListener(() => HideAndShow(firstMenuContainer, multiplayerMenuContainer));
        multiplayerStartGameBtn.onClick.AddListener(() => gameConsole.StartOnlineGame());
        multiplayerBackBtn.onClick.AddListener(() =>
        {
            OnClick_MultiplayerBackBtn();
            HideAndShow(multiplayerMenuContainer, firstMenuContainer);
        });
        multiplayerCreateRoomBtn.onClick.AddListener(() =>
        {
            gameConsole.CreateRoom();
            HideAndShow(multiplayerCreateOrJoinMenu, multiplayerRoomMenu);
        });
        multiplayerJoinGameBtn.onClick.AddListener(() =>
        {
            UpdateRoomList();
            HideAndShow(multiplayerCreateOrJoinMenu, multiplayerRoomListMenu);
        });

        onePlayerMenuContainer.gameObject.SetActive(false);
        multiplayerMenuContainer.gameObject.SetActive(false);
    }

    private void HideAndShow(Transform transformToHide, Transform transformToShow)
    {
        transformToHide.gameObject.SetActive(false);
        transformToShow.gameObject.SetActive(true);
    }
    private void UpdateRoomList()
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(currentGameSO.gameId.ToString(), out object value))
        {
            string roomListJson = (string)value;
            GameRoomList gameRooms = JsonHelper<GameRoomList>.Deserialize(roomListJson);
            UpdateRoomList(gameRooms);
        }
    }
    private void UpdateRoomList(GameRoomList gameRooms)
    {
        for (int i = 0; i < roomListButtons.Count; i++)
        {
            Transform btnTransform = roomListButtons[i];
            Destroy(btnTransform.gameObject);
        }
        roomListButtons.Clear();
        // Gerekli olan elementler: Odalarýn adý, min max oyuncu sayýsý
        foreach (GameRoom room in gameRooms.roomList)
        {
            if (room.currentPlayerCount >= room.maxPlayerCount)
                continue;
            Transform roomListBtnTransform = Instantiate(roomListButtonPrefab, roomListButtonsContainer);
            roomListBtnTransform.gameObject.SetActive(true);
            roomListBtnTransform.GetComponent<MultiplayerGameRoomListButton>().Initialize(room.roomName, room.maxPlayerCount);
            Button roomListBtn = roomListBtnTransform.GetComponentInChildren<Button>();
            roomListBtn.onClick.AddListener(() => OnClick_JoinRoom(room));
            roomListButtons.Add(roomListBtnTransform);
        }
    }
    public void UpdateRoom(GameRoom updatedRoom, GameRoom joinedRoom)
    {
        if (updatedRoom.roomHostId != joinedRoom.roomHostId)
            return;

        for (int i = 0; i < roomMembers.Count; i++)
        {
            Transform roomMember = roomMembers[i];
            Destroy(roomMember.gameObject);
        }
        roomMembers.Clear();
        // Gerekli olan elementler: Odalarýn adý, min max oyuncu sayýsý
        foreach (var memberId in updatedRoom.roomMembersIds)
        {
            if (PhotonHandler.Instance.TryGetPlayerByActorNumber(memberId, out var player))
            {
                Transform roomMemberTransform = Instantiate(roomMemberPrefab, roomMembersContainer);
                roomMemberTransform.gameObject.SetActive(true);
                roomMemberTransform.GetComponent<TextMeshProUGUI>().text = player.NickName;
                roomMembers.Add(roomMemberTransform);
            }
        }
        if (updatedRoom.roomHostId == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            multiplayerStartGameBtn.enabled = updatedRoom.currentPlayerCount >= 1;
        }

    }
    private void OnClick_JoinRoom(GameRoom currentRoom)
    {
        if (!PhotonHandler.Instance.TryGetPlayerByActorNumber(currentRoom.roomHostId, out Player pl))
        {
            UpdateRoomList(); return;
        }

        HideAndShow(multiplayerRoomListMenu, multiplayerRoomMenu);
        multiplayerStartGameBtn.gameObject.SetActive(false);
        waitingHostToStartTMP.gameObject.SetActive(true);
        gameConsole.OnJoinedGameRoom();

        string roomListJson = (string)PhotonNetwork.CurrentRoom.CustomProperties[currentGameSO.gameId.ToString()];
        GameRoomList gameRooms = JsonHelper<GameRoomList>.Deserialize(roomListJson);
        GameRoom room = gameRooms.FindRoomByHostId(currentRoom.roomHostId);
        room.currentPlayerCount++;
        room.roomMembersIds.Add(PhotonNetwork.LocalPlayer.ActorNumber);

        roomListJson = JsonHelper<GameRoomList>.Serialize(gameRooms);
        PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable { { currentGameSO.gameId.ToString(), roomListJson } });
        gameConsole.joinedRoom = currentRoom;
    }
    private void OnClick_MultiplayerBackBtn()
    {
        if (multiplayerRoomMenu.gameObject.activeSelf)
        {
            gameConsole.OnLeftGameRoom();
            HideAndShow(multiplayerRoomMenu, multiplayerCreateOrJoinMenu);
        }
        else
            HideAndShow(multiplayerRoomListMenu, multiplayerCreateOrJoinMenu);
    }
    public void OnHostLeftRoom()
    {
        HideAndShow(multiplayerRoomMenu, multiplayerCreateOrJoinMenu);
    }
    public void OnCreatedRoom()
    {
        multiplayerStartGameBtn.gameObject.SetActive(true);
        waitingHostToStartTMP.gameObject.SetActive(false);
        multiplayerStartGameBtn.enabled = false;
    }
    public void AdjustScreen(GameSO gameSO)
    {
        currentGameSO = gameSO;
        gameNameTMP.text = gameSO.gameName;
        multiplayerRoomInfoText.text = "Players: (Min:2, Max:" + gameSO.maxPlayer + ")";
    }
    public void OnStartedToOfflineGame()
    {
        HideAndShow(onePlayerMenuContainer, firstMenuContainer);
    }
}
