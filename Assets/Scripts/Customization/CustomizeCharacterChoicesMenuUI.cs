using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomizeCharacterChoicesMenuUI : MonoBehaviour
{
    public static CustomizeCharacterChoicesMenuUI Instance;
    [SerializeField] private Button customizationPartButton;
    [SerializeField] private Transform contentRect;
    private List<Transform> customizationButtonTransforms = new List<Transform>();
    private void Awake()
    {
        Instance = this;
    }
    public void ShowChoices(CustomizationContent content, CustomizationPart part)
    {
        CustomizationSO customizationSO = PlayerCustomizer.Instance.GetCustomizationSO(content, part);

        if (customizationSO is null)
            CleanCustomizationButtons();
        else
            UpdateContent(customizationSO);
    }
    private void UpdateContent(CustomizationSO customizationSO)
    {
        CleanCustomizationButtons();

        for (int i = 0; i < customizationSO.customizationParts.Count; i++)
        {
            var customizationPart = customizationSO.customizationParts[i];
            Button customizationBtn = Instantiate(customizationPartButton, contentRect);
            customizationBtn.onClick.AddListener(() => OnClick_CustomizationPartButton(customizationSO, customizationPart.customizationPart));
            customizationBtn.GetComponent<CustomizationPartButton>().InitializeButton(customizationPart.customizationPartSprite);
            customizationButtonTransforms.Add(customizationBtn.transform);
        }
    }
    private void CleanCustomizationButtons()
    {
        //Clean previous content
        for (int i = 0; i < customizationButtonTransforms.Count; i++)
        {
            Transform button = customizationButtonTransforms[i];
            Destroy(button.gameObject);
        }
        customizationButtonTransforms.Clear();
    }
    private void OnClick_CustomizationPartButton(CustomizationSO customizationSO, Transform customizationTransform)
    {
        PlayerCustomizer.Instance.Customize(customizationSO.content, customizationSO.part, customizationTransform);
    }
}
