using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class DrawingGenerator : MonoBehaviour
{
    [SerializeField] private Transform pixelContainer;
    [SerializeField] private ColorPickerUI colorPickerUI;
    private Color currentColor;
    private void Awake()
    {
        currentColor = Color.white;
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
        Debug.Log(argsJson + "\n\n\n");
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
}
