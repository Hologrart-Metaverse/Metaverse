using UnityEngine;
using UnityEngine.UI;

public class CustomizationPartButton : MonoBehaviour
{
    [SerializeField] private Image customizationPartImage;
    public void InitializeButton(Sprite sprite)
    {
        customizationPartImage.sprite = sprite;
    } 
}
