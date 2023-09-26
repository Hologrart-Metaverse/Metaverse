using UnityEngine;
public enum CustomizationContent
{
    Body,
    Clothes
}
public enum CustomizationPart
{
    //Body
    BodyType,
    HairStyle,
    HairColor,
    EyeShape,
    EyeColor,

    //Clothes
    Clothes,
    Earrings,
    Headwears,

}
public class PlayerCustomizer : MonoBehaviour
{
    public static PlayerCustomizer Instance;
    [SerializeField] private AllCustomizationSOs customizations;

    private void Awake()
    {
        Instance = this;
    }
    [SerializeField] private Model model;

    public void SetModel(Model model)
    {
        this.model = model;
    }
    public CustomizationSO GetCustomizationSO(CustomizationContent content, CustomizationPart part)
    {
        foreach (var customization in customizations.customizationSOs)
        {
            if (customization.content == content && customization.part == part)
            {
                return customization;
            }
        }
        return null;
    }
    public void Customize(CustomizationContent content, CustomizationPart part, Transform customizationTransform)
    {
        switch (content)
        {
            case CustomizationContent.Body:
                switch (part)
                {
                    case CustomizationPart.BodyType:
                        break;
                    case CustomizationPart.HairStyle:
                        break;
                    case CustomizationPart.HairColor:
                        break;
                    case CustomizationPart.EyeShape:
                        break;
                    case CustomizationPart.EyeColor:
                        break;
                }
                break;

            case CustomizationContent.Clothes:
                switch (part)
                {
                    case CustomizationPart.Clothes:
                        CustomizeClothes(customizationTransform, Model.ModelPart.Top);
                        break;
                    case CustomizationPart.Earrings:
                        break;
                    case CustomizationPart.Headwears:
                        break;
                }
                break;
        }
    }
    private void CustomizeClothes(Transform customizationTransform, Model.ModelPart modelPart)
    {
        Transform modelPartTransform = model.GetModelPart(modelPart);
        switch (modelPart)
        {
            case Model.ModelPart.Top:
                modelPartTransform.GetComponent<MeshFilter>().sharedMesh = customizationTransform.GetComponentInChildren<MeshFilter>().sharedMesh;
                modelPartTransform.GetComponent<MeshRenderer>().sharedMaterials = customizationTransform.GetComponentInChildren<MeshRenderer>().sharedMaterials;
                break;
                
        }
    }
}
