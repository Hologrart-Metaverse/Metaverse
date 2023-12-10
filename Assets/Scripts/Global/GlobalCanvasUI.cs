using Photon.Pun;
using TMPro;
using UnityEngine;

public class GlobalCanvasUI : MonoBehaviour
{
    public static GlobalCanvasUI Instance { get; private set; }
    [SerializeField] private TextMeshProUGUI nicknameTMP;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        StateHandler.Instance.StateChanged += StateHandler_StateChanged;
        nicknameTMP.text = PhotonNetwork.NickName;
    }
    private void StateHandler_StateChanged(StateHandler sender, State state)
    {
        switch (state)
        {
            case State.None:
                ShowChildren();
                break;
            default:
                HideChildren();
                break;
        }
    }
    public void HideChildren()
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }
    public void ShowChildren()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }
    }
}
