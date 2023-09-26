using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomizeCharacterOptionsUI : MonoBehaviour
{
    [Tooltip("You should serialize content buttons in same order with content transforms")]
    [SerializeField] private List<Button> contentButtons;
    [SerializeField] private List<Transform> contentTransforms;
    private Transform lastContent;
    private void Start()
    {
        for (int i = 0; i < contentButtons.Count; i++)
        {
            int order = i;
            Button btn = contentButtons[order];
            btn.onClick.AddListener(() => ShowContent(order));
        }
        lastContent = contentTransforms[0];
    }
    private void ShowContent(int order)
    {
        if (lastContent != null)
        {
            lastContent.gameObject.SetActive(false);
        }
        Transform currentContent = contentTransforms[order];
        currentContent.gameObject.SetActive(true);
        lastContent = currentContent;
    }
}
