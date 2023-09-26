using UnityEngine;
using UnityEngine.UI;

public class CustomizeMenuUI : MonoBehaviour
{
    private CustomizeModelPositionHandler modelPositionHandler;
    [SerializeField] private Transform titleContainer, customizeScreen;
    [SerializeField] private Button backBtn;
    private void Start()
    {
        backBtn.onClick.AddListener(() => CloseMenu());
        modelPositionHandler = GetComponent<CustomizeModelPositionHandler>();
        SetModelState(CustomizeModelPositionHandler.CustomizeState.Menu);
        customizeScreen.gameObject.SetActive(false);
    }
    public void OpenMenu()
    {
        titleContainer.gameObject.SetActive(false);
        SetModelState(CustomizeModelPositionHandler.CustomizeState.BodyCustomize);
        customizeScreen.gameObject.SetActive(true);
    }
    private void CloseMenu()
    {
        titleContainer.gameObject.SetActive(true);
        SetModelState(CustomizeModelPositionHandler.CustomizeState.Menu);
        customizeScreen.gameObject.SetActive(false);
    }
    public void SetModelState(CustomizeModelPositionHandler.CustomizeState state)
    {
        modelPositionHandler.SetPosition(state);
    }
}
