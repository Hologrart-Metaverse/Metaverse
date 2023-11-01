using Photon.Pun;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class JointNFT : MonoBehaviour, I_UIModeChanger
{
    private Cinemachine.CinemachineVirtualCamera screenCam;
    [SerializeField] private Transform screenSurface;
    [SerializeField] private Transform UI;
    [SerializeField] private ColorPickerUI colorPickerUI;
    [SerializeField] private PixelCreator pixelCreator;
    private Image currentPixel;
    private int currentPixelIndex;
    private Color currentColor;
    private Color pickedColor = default;
    private PhotonView PV;

    private void Awake()
    {
        UI.gameObject.SetActive(false);
        PV = GetComponent<PhotonView>();
        colorPickerUI.gameObject.SetActive(false);
    }
    private void Start()
    {
        screenCam = GlobalCameraManager.Instance.GetVirtualCamera(GlobalCameraManager.CameraType.Details);
    }
    public void ChangeMode(UIMode mode)
    {
        switch (mode)
        {
            case UIMode.StaticEdit:
                screenCam.Follow = screenSurface;
                screenCam.transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y - 90, 0);
                screenCam.enabled = true;
                Utils.SetMouseLockedState(false);
                StateHandler.Instance.SetState(State.ArtPlanet_StaticUI);
                UI.gameObject.SetActive(true);
                ZoomSystem.Instance.StartZooming(ZoomSystem.ZoomMode.Static, ZoomSystem.ZoomSurface.JointNft);
                break;
            default:
                screenCam.enabled = false;
                screenCam.Follow = null;
                Utils.SetMouseLockedState(true);
                StateHandler.Instance.SetState(State.None);
                UI.gameObject.SetActive(false);
                EventSystem.current.SetSelectedGameObject(null);
                ZoomSystem.Instance.EndZooming();
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
        colorHex = "#" + colorHex.Replace("HEX", "");
        if (ColorUtility.TryParseHtmlString(colorHex, out Color color))
        {
            pixel.color = color;
        }
    }
    public void OnClick_BackButton()
    {
        ChangeMode(UIMode.None);
    }
    public void ResetView()
    {
        ZoomSystem.Instance.ResetView();
    }
}
