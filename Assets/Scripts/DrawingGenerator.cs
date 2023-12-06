using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class DrawingGenerator : MonoBehaviour
{
    [SerializeField] private Transform pixelContainer;
    [SerializeField] private TMP_Dropdown widthAndHeightDropdown;
    [SerializeField] private ColorPickerUI colorPickerUI;
    [SerializeField] private Transform pixelPrefab;
    [SerializeField] private TextMeshProUGUI copiedTMP;
    private Color currentColor;
    private void Awake()
    {
        currentColor = Color.white;
        copiedTMP.enabled = false;
    }
    public void GenerateDrawing()
    {
        PixelArgs args = new PixelArgs();
        args.pixels = new Dictionary<int, string>();
        for (int i = 0; i < pixelContainer.childCount; i++)
        {
            if (!pixelContainer.GetChild(i).gameObject.activeSelf)
                continue;
            string pixelHTML = "#" + ColorUtility.ToHtmlStringRGB(pixelContainer.GetChild(i).GetComponent<Image>().color);
            args.pixels.Add(i, pixelHTML);
        }
        string argsJson = JsonHelper<PixelArgs>.Serialize(args);
        GUIUtility.systemCopyBuffer = argsJson;
        copiedTMP.enabled = true;
        Invoke(nameof(DisableCopiedTMP), 2f);
    }
    private void DisableCopiedTMP()
    {
        copiedTMP.enabled = false;
    }
    public void OnColorChanged(Color color)
    {
        currentColor = color;
    }
    public void OnClick_Pixel(Button button)
    {
        button.GetComponent<Image>().color = currentColor;
        colorPickerUI.CreatePickedColorPrefab(currentColor);
    }
    public void OnClickImage(Image image)
    {
        currentColor = image.color;
    }
    public void OnDropdownValueChanged()
    {
        float i = float.Parse(widthAndHeightDropdown.options[widthAndHeightDropdown.value].text.Split(' ')[0]);
        pixelContainer.GetComponent<GridLayoutGroup>().cellSize = new Vector2(1000 / i, 1000 / i);
        AdjustPixels((int)(i * i));
    }
    private void AdjustPixels(int pixelCount)
    {
        if(pixelContainer.childCount > pixelCount)
        {
            for(int i = pixelContainer.childCount - 1; i >= pixelCount; i--)
            {
                Destroy(pixelContainer.GetChild(i).gameObject);
            }
        }
        else
        {
            for (int i = pixelContainer.childCount; i < pixelCount; i++)
            {
                Instantiate(pixelPrefab, pixelContainer);
            }
        }
        for(int i = 0; i< pixelContainer.childCount; i++)
        {
            pixelContainer.GetChild(i).GetComponent<Image>().color = Color.white;
        }
    }
}
