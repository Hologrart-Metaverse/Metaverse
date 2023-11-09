using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TeleportUI : MonoBehaviour
{
    public static TeleportUI Instance { get; private set; }
    [SerializeField] private Transform teleportUI;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private Button areaBtnPrefab;
    [SerializeField] private Transform contentRect;
    private List<Button> areaBtns = new List<Button>();
    private float contentY;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        Hide();
        contentY = contentRect.localPosition.y;
        Debug.Log(contentY);
    }
    public void OnScrollValueChanged()
    {
        int areaBtnsOrder = areaBtns.Count > 5 ? areaBtns.Count - 5 : 0;

        //Clamp content position
        Vector3 contentPos = contentRect.localPosition;
        contentPos.y = Mathf.Clamp(contentPos.y, contentY, contentY + areaBtnsOrder * 30);
        contentRect.localPosition = contentPos;
    }
    public void Show()
    {
        UpdateContentVisual();
        teleportUI.gameObject.SetActive(true);
        Utils.SetMouseLockedState(false);
        Utils.IsUIScreenOpen = true;
    }
    public void Hide()
    {
        teleportUI.gameObject.SetActive(false);
        Utils.SetMouseLockedState(true);
        Utils.IsUIScreenOpen = false;
        EventSystem.current.SetSelectedGameObject(null);
    }
    private void UpdateContentVisual()
    {
        //Clear content
        for (int i = 0; i < areaBtns.Count; i++)
        {
            Destroy(areaBtns[i].gameObject);
        }
        areaBtns.Clear();

        List<Area> areaList = AreaSystem.Instance.GetAllAreasExceptCurrent();

        for (int i = 0; i < areaList.Count; i++)
        {
            Button areaBtn = Instantiate(areaBtnPrefab, contentRect);
            Area area = areaList[i];
            string areaName = area.ToString();
            if (areaName.Contains('_'))
            {
                areaName = areaName.Replace('_', ' ');
            }
            areaBtn.GetComponent<AreaButton>().InitializeButton(areaName);
            areaBtn.onClick.AddListener(() => OnClickAreaBtn(area));
            areaBtns.Add(areaBtn);
        }
    }
    private void OnClickAreaBtn(Area area)
    {
        TeleportSystem.Instance.TeleportArea(area);
        Hide();
    }

}
