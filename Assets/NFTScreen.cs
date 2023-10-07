using Photon.Pun;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NFTScreen : MonoBehaviour
{
    [SerializeField] private Camera screenCam;
    [SerializeField] private Transform UI;
    [SerializeField] private ColorPickerUI colorPickerUI;
    [SerializeField] private PixelCreator pixelCreator;
    private Image currentPixel;
    private int currentPixelIndex;
    private Color currentColor;
    private Color pickedColor = default;
    private PhotonView PV;
    public enum Mode
    {
        Edit,
        None,
    }
    private void Awake()
    {
        UI.gameObject.SetActive(false);
        PV = GetComponent<PhotonView>();
        colorPickerUI.gameObject.SetActive(false);
    }
    public void ChangeMod(Mode mode)
    {
        switch (mode)
        {
            case Mode.Edit:
                screenCam.enabled = true;
                Utils.SetMouseLockedState(false);
                StateHandler.Instance.SetState(State.EditingNFTScreen);
                UI.gameObject.SetActive(true);
                break;
            case Mode.None:
                screenCam.enabled = false;
                Utils.SetMouseLockedState(true);
                StateHandler.Instance.SetState(State.None);
                UI.gameObject.SetActive(false);
                EventSystem.current.SetSelectedGameObject(null);
                break;
        }
    }
    public void OnClick_Pixel(int index)
    {
        colorPickerUI.gameObject.SetActive(true);
        Debug.Log("CLICKED " + index + ". PIXEL");
        if (currentPixel != null)
        {
            currentPixel.color = currentColor;
        }
        currentPixelIndex = index;
        Image pixel = pixelCreator.GetPixel(currentPixelIndex);
        currentPixel = pixel;
        currentColor = pixel.color;
        if (pickedColor != default)
            pixel.color = pickedColor;
    }
    public void OnColorChanged(Color color)
    {
        if (currentPixel == null)
            return;

        pickedColor = color;
        currentPixel.color = pickedColor;
    }
    public void OnColorPicked()
    {
        //SAVE CURRENT PIXEL AND COLOR HERE

        string colorHex = ColorUtility.ToHtmlStringRGB(pickedColor);
        colorPickerUI.CreatePickedColorPrefab(pickedColor);
        PV.RPC(nameof(ChangePixelColorRPC), RpcTarget.OthersBuffered, currentPixelIndex, colorHex);
        currentPixel = null;
    }
    [PunRPC]
    private void ChangePixelColorRPC(int index, string colorHex)
    {
        Image pixel = pixelCreator.GetPixel(index);
        colorHex = "#" + colorHex.Replace("HEX","");
        if (ColorUtility.TryParseHtmlString(colorHex, out Color color))
        {
            pixel.color = color;
        }
    }
    public void OnClick_BackButton()
    {
        ChangeMod(Mode.None);
    }
}
