using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteractionUI : MonoBehaviour
{
    public static InteractionUI Instance { get; set; }
    [SerializeField] private Transform UI;
    [SerializeField] private TextMeshProUGUI interactionTMP;
    [SerializeField] private Button interactionBtn; //This button can be used on mobile. It's not necessary on PC.
    private void Awake()
    {
        Instance = this;
        HideText();
    }
    public void ShowText(string text)
    {
        interactionTMP.text = text;
        UI.gameObject.SetActive(true);
    }
    public void HideText()
    {
        UI.gameObject.SetActive(false);
    }
}
