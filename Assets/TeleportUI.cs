using System;
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
        //Clamp content position
        Vector3 contentPos = contentRect.localPosition;
        Debug.Log(contentPos.y);
        contentPos.y = Mathf.Clamp(contentPos.y, contentY, contentY + (areaBtns.Count - 5) * 30);
        contentRect.localPosition = contentPos;
    }
    public void Show()
    {
        UpdateContentVisual();
        teleportUI.gameObject.SetActive(true);
        Utils.SetMouseLockedState(false);
        Utils.IsChooseScreenOn = true;
    }
    public void Hide()
    {
        teleportUI.gameObject.SetActive(false);
        Utils.SetMouseLockedState(true);
        Utils.IsChooseScreenOn = false;
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

        List<Areas> areaList = TeleportSystem.Instance.GetSuitableAreas();

        for (int i = 0; i < areaList.Count; i++)
        {
            Button areaBtn = Instantiate(areaBtnPrefab, contentRect);
            Areas area = areaList[i];
            areaBtn.GetComponent<AreaButton>().InitializeButton(area.ToString());
            areaBtn.onClick.AddListener(() => OnClickAreaBtn(area));
            areaBtns.Add(areaBtn);
        }
    }
    private void OnClickAreaBtn(Areas area)
    {
        TeleportSystem.Instance.Teleport(area);
        Hide();
    }

}
