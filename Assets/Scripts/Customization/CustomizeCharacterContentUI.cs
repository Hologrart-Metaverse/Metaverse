using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class CustomizeCharacterContentArgs
{
    public CustomizationContent content;
    public CustomizationPart part;
    public Button partButton;
}
public class CustomizeCharacterContentUI : MonoBehaviour
{
    private CustomizeModelPositionHandler modelPositionHandler;
    [Header("All parts of under this content must be in this list")]
    [SerializeField] private List<CustomizeCharacterContentArgs> contentArgs = new List<CustomizeCharacterContentArgs>();

    private void Awake()
    {
        foreach (var ct in contentArgs)
        {
            ct.partButton.onClick.AddListener(() => ShowContent(ct.content, ct.part));
        }
        modelPositionHandler = transform.GetComponentInParent<CustomizeModelPositionHandler>();
    }
    private void ShowContent(CustomizationContent content, CustomizationPart part)
    {
        CustomizeCharacterChoicesMenuUI.Instance.ShowChoices(content, part);
        ChangeModelPosition(content, part);
    }

    private void ChangeModelPosition(CustomizationContent content, CustomizationPart part)
    {
        switch (content)
        {
            case CustomizationContent.Body:
                switch (part)
                {
                    case CustomizationPart.BodyType:
                        modelPositionHandler.SetPosition(CustomizeModelPositionHandler.CustomizeState.BodyCustomize);
                        break;
                    case CustomizationPart.HairStyle:
                        modelPositionHandler.SetPosition(CustomizeModelPositionHandler.CustomizeState.FaceCustomize);
                        break;
                    case CustomizationPart.HairColor:
                        modelPositionHandler.SetPosition(CustomizeModelPositionHandler.CustomizeState.FaceCustomize);
                        break;
                    case CustomizationPart.EyeShape:
                        modelPositionHandler.SetPosition(CustomizeModelPositionHandler.CustomizeState.FaceCustomize);
                        break;
                    case CustomizationPart.EyeColor:
                        modelPositionHandler.SetPosition(CustomizeModelPositionHandler.CustomizeState.FaceCustomize);
                        break;
                }
                break;

            case CustomizationContent.Clothes:
                switch (part)
                {
                    case CustomizationPart.Clothes:
                        modelPositionHandler.SetPosition(CustomizeModelPositionHandler.CustomizeState.BodyCustomize);
                        break;
                    case CustomizationPart.Earrings:
                        modelPositionHandler.SetPosition(CustomizeModelPositionHandler.CustomizeState.FaceCustomize);
                        break;
                    case CustomizationPart.Headwears:
                        modelPositionHandler.SetPosition(CustomizeModelPositionHandler.CustomizeState.FaceCustomize);
                        break;
                }
                break;
        }
    }
}
