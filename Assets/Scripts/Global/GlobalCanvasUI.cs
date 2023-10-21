using UnityEngine;

public class GlobalCanvasUI : MonoBehaviour
{
    public static GlobalCanvasUI Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        StateHandler.Instance.StateChanged += StateHandler_StateChanged;
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
