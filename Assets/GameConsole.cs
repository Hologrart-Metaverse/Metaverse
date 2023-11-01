using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Hashtable = ExitGames.Client.Photon.Hashtable;
public class GameConsole : MonoBehaviourPunCallbacks, I_Interactable
{
    public GameSO gameSO;
    private GameConsoleScreenUI consoleScreenUI;
    private PhotonView PV;
    private bool didICreateRoom;
    private bool didIJoinedRoom;
    private void Awake()
    {
        PV = GetComponent<PhotonView>();

        consoleScreenUI = GetComponentInChildren<GameConsoleScreenUI>();
        consoleScreenUI.AdjustScreen(gameSO);
    }
    public void Interact() { }

    public void OnFaced()
    {
        Utils.SetMouseLockedState(false);
    }

    public void OnInteractEnded()
    {
        Utils.SetMouseLockedState(true);
        EventSystem.current.SetSelectedGameObject(null);
    }
    public void CreateRoom()
    {
        didICreateRoom = true;
        string gameRoomListJson = "";
        GameRoomList gameRooms;
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(gameSO.gameId.ToString(), out object value))
        {
            gameRoomListJson = (string)value;
            gameRooms = JsonHelper<GameRoomList>.Deserialize(gameRoomListJson);
        }
        else
        {
            gameRooms = new();
            gameRooms.roomList = new();
        }

        GameRoom newRoom = new()
        {
            roomName = PhotonNetwork.LocalPlayer.NickName + "'s Room",
            roomHostId = PhotonNetwork.LocalPlayer.ActorNumber,
            roomMembersIds = new() { PhotonNetwork.LocalPlayer.ActorNumber },
            currentPlayerCount = 1,
            maxPlayerCount = gameSO.maxPlayer,
        };
        gameRooms.roomList.Add(newRoom);
        consoleScreenUI.OnCreatedRoom(newRoom);
        gameRoomListJson = JsonHelper<GameRoomList>.Serialize(gameRooms);
        PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable { { gameSO.gameId.ToString(), gameRoomListJson } });

    }
    public override void OnLeftRoom()
    {
        if (didICreateRoom)
        {
            //Remove room from room properties
        }
    }
    public void OnJoinedGameRoom()
    {
        didIJoinedRoom = true;
    }
    public void OnLeftGameRoom()
    {
        didIJoinedRoom = false;
        didICreateRoom = false;
    }
    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey(gameSO.gameId.ToString()) && (didICreateRoom || didIJoinedRoom))
        {
            string roomListJson = (string)PhotonNetwork.CurrentRoom.CustomProperties[gameSO.gameId.ToString()];
            Debug.Log(roomListJson);
            GameRoomList gameRooms = JsonHelper<GameRoomList>.Deserialize(roomListJson);
            GameRoom gameRoom = gameRooms.FindMyRoom(PhotonNetwork.LocalPlayer.ActorNumber);
            if (gameRoom == null)
            {
                //Host left the game
                consoleScreenUI.OnHostLeftRoom();
                return;
            }

            consoleScreenUI.UpdateRoom(gameRoom);
        }
    }
}

[System.Serializable]
public class GameRoomList
{
    public List<GameRoom> roomList;

    public GameRoom FindRoomByHostId(int hostId)
    {
        foreach (var room in roomList)
        {
            if (room.roomHostId == hostId)
                return room;
        }
        return null;
    }
    public GameRoom FindMyRoom(int actorNumber)
    {
        foreach (var room in roomList)
        {
            foreach (var memberId in room.roomMembersIds)
            {
                if (memberId == actorNumber)
                    return room;
            }
        }
        return null;
    }
}
[System.Serializable]
public class GameRoom
{
    public string roomName { get; set; }
    public int roomHostId { get; set; }
    public List<int> roomMembersIds { get; set; }
    public int currentPlayerCount;
    public int maxPlayerCount;

}