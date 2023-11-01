using TMPro;
using UnityEngine;

public class MultiplayerGameRoomListButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI roomNameTMP;
    [SerializeField] private TextMeshProUGUI playerCountTMP;

    public void Initialize(string roomName, int maxPlayer)
    {
        roomNameTMP.text = roomName;
        playerCountTMP.text = "1/" + maxPlayer;
    }
    public void UpdatePlayerCount(int currentPlayerCount)
    {
        playerCountTMP.text = currentPlayerCount.ToString() + playerCountTMP.text[1..];
    }
}
