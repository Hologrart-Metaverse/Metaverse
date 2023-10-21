using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PixelCreator : MonoBehaviour
{
    public Transform pixelPrefab;
    public Transform container;
    private Dictionary<int, Image> pixelDict = new Dictionary<int, Image>();
    private JointNFT nftScreen;
    private void Awake()
    {
        nftScreen = GetComponentInParent<JointNFT>();
    }
    void Start()
    {
        for (int i = 0; i < 64 * 64; i++)
        {
            int index = i;  
            Transform pixelTransform = Instantiate(pixelPrefab, container);
            pixelTransform.GetComponent<Button>().onClick.AddListener(() => nftScreen.OnClick_Pixel(index));
            pixelDict.Add(index, pixelTransform.GetComponent<Image>());
        }
    }
    public Image GetPixel(int index)
    {
        return pixelDict[index];
    }
}
