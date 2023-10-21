using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ColorPickerUI : MonoBehaviour
{
    private JointNFT nftScreen;
    [SerializeField] private Transform pickedColorsContainer;
    [SerializeField] private Transform pickedColorPrefab;
    private List<Color> colorList = new List<Color>();
    private void Start()
    {
        nftScreen = GetComponentInParent<JointNFT>();
    }
    public void OnColorChanged(Color color)
    {
        nftScreen.OnColorChanged(color);
    }
    public void CreatePickedColorPrefab(Color color)
    {
        if(colorList.Contains(color)) { return; }

        Transform pickedColorTrnsfrm = Instantiate(pickedColorPrefab, pickedColorsContainer);
        pickedColorTrnsfrm.gameObject.SetActive(true);
        Image pickedColorImg = pickedColorTrnsfrm.GetComponent<Image>();
        pickedColorImg.color = color;
        pickedColorTrnsfrm.GetComponent<Button>().onClick.AddListener(() => nftScreen.OnColorChanged(pickedColorImg.color));
        colorList.Add(color);
    }
    public void OnClick_OK()
    {
        nftScreen.OnColorPicked();
        gameObject.SetActive(false);
    }
}
