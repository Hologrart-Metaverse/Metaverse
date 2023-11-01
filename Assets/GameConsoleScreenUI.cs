using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class GameConsoleScreenUI : MonoBehaviour
{
    [SerializeField] private Transform firstMenuContainer;
    [SerializeField] private Transform onePlayerMenuContainer;
    [SerializeField] private Transform multiplayerMenuContainer;
    [SerializeField] private Transform multiplayerCreateOrJoinMenu;
    [SerializeField] private Transform multiplayerRoomMenu;
    [SerializeField] private Transform multiplayerRoomListMenu;
    [SerializeField] private Button onePlayerEnterBtn;
    [SerializeField] private Button onePlayerBackBtn;
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
    private GameRoom joinedRoom;
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
        multiplayerEnterBtn.onClick.AddListener(() => HideAndShow(firstMenuContainer, multiplayerMenuContainer));
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
    public void UpdateRoom(GameRoom room)
    {
        if (room.roomHostId != joinedRoom.roomHostId)
            return;
        Debug.Log("odada " + roomMembers.Count + " oyuncu vardý");

        for (int i = 0; i < roomMembers.Count; i++)
        {
            Transform roomMember = roomMembers[i];
            Destroy(roomMember.gameObject);
        }
        roomMembers.Clear();
        // Gerekli olan elementler: Odalarýn adý, min max oyuncu sayýsý
        foreach (var memberId in room.roomMembersIds)
        {
            Debug.Log("member id: " + memberId);
            Transform roomMemberTransform = Instantiate(roomMemberPrefab, roomMembersContainer);
            roomMemberTransform.gameObject.SetActive(true);
            roomMemberTransform.GetComponent<TextMeshProUGUI>().text = PhotonHandler.Instance.GetPlayerByActorNumber(memberId).NickName;
            roomMembers.Add(roomMemberTransform);
        }
        if (room.roomHostId == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            multiplayerStartGameBtn.enabled = room.currentPlayerCount >= 2;
        }

    }
    private void OnClick_JoinRoom(GameRoom currentRoom)
    {
        HideAndShow(multiplayerRoomListMenu, multiplayerRoomMenu);
        multiplayerStartGameBtn.gameObject.SetActive(false);
        waitingHostToStartTMP.gameObject.SetActive(true);
        gameConsole.OnJoinedGameRoom();
        gameHost = PhotonHandler.Instance.GetPlayerByActorNumber(currentRoom.roomHostId);
        Debug.Log("HOST: " + gameHost.NickName);

        string roomListJson = (string)PhotonNetwork.CurrentRoom.CustomProperties[currentGameSO.gameId.ToString()];
        GameRoomList gameRooms = JsonHelper<GameRoomList>.Deserialize(roomListJson);
        GameRoom room = gameRooms.FindRoomByHostId(currentRoom.roomHostId);
        room.currentPlayerCount++;
        room.roomMembersIds.Add(PhotonNetwork.LocalPlayer.ActorNumber);

        roomListJson = JsonHelper<GameRoomList>.Serialize(gameRooms);
        PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable { { currentGameSO.gameId.ToString(), roomListJson } });
        joinedRoom = currentRoom;
    }
    private void OnClick_MultiplayerBackBtn()
    {
        if (multiplayerRoomMenu.gameObject.activeSelf)
        {
            OnLeftRoom();
            HideAndShow(multiplayerRoomMenu, multiplayerCreateOrJoinMenu);
        }
        else
            HideAndShow(multiplayerRoomListMenu, multiplayerCreateOrJoinMenu);
    }
    private void OnLeftRoom()
    {
        if (joinedRoom.roomHostId == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            DeleteRoom();
        }
        else
        {
            LeaveRoom();
        }
    }

    private void DeleteRoom()
    {
        string roomListJson = (string)PhotonNetwork.CurrentRoom.CustomProperties[currentGameSO.gameId.ToString()];
        GameRoomList gameRooms = JsonHelper<GameRoomList>.Deserialize(roomListJson);
        GameRoom room = gameRooms.FindRoomByHostId(joinedRoom.roomHostId);

        gameRooms.roomList.Remove(room);
        roomListJson = JsonHelper<GameRoomList>.Serialize(gameRooms);

        PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable { { currentGameSO.gameId.ToString(), roomListJson } });
        joinedRoom = null;
        gameConsole.OnLeftGameRoom();
    }
    private void LeaveRoom()
    {
        string roomListJson = (string)PhotonNetwork.CurrentRoom.CustomProperties[currentGameSO.gameId.ToString()];
        GameRoomList gameRooms = JsonHelper<GameRoomList>.Deserialize(roomListJson);
        GameRoom room = gameRooms.FindRoomByHostId(joinedRoom.roomHostId);

        room.roomMembersIds.Remove(PhotonNetwork.LocalPlayer.ActorNumber);
        room.currentPlayerCount--;

        roomListJson = JsonHelper<GameRoomList>.Serialize(gameRooms);
        PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable { { currentGameSO.gameId.ToString(), roomListJson } });
        joinedRoom = null;
        gameConsole.OnLeftGameRoom();
    }
    public void OnHostLeftRoom()
    {
        joinedRoom = null;
        HideAndShow(multiplayerRoomMenu, multiplayerCreateOrJoinMenu);
        gameConsole.OnLeftGameRoom();
    }
    public void OnCreatedRoom(GameRoom room)
    {
        joinedRoom = room;
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
}
