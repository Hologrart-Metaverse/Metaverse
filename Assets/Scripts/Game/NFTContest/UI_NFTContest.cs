using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class PixelDrawingArgs
{
    public Sprite pixelDrawing;
    [TextArea(2, 5)]
    public string drawingArgsJson;
}
[Serializable]
public class PixelArgs
{
    public Dictionary<int, string> pixels;
}
public class UI_NFTContest : GameUI
{
    [SerializeField] private List<PixelDrawingArgs> args;
    [SerializeField] private Transform pixelContainer;
    [SerializeField] private Transform pixelPrefab;
    [SerializeField] private Transform availableColorsContainer;
    [SerializeField] private Transform availableColorPrefab;
    [SerializeField] private Image targetDrawingImage;
    private List<string> availableColors = new List<string>();
    private List<Transform> availableColorPrefabs = new List<Transform>();
    private List<Button> pixelBtns = new List<Button>();
    private PixelArgs currentPixelArgs = new();
    private PixelArgs targetPixelArgs;
    private Color currentColor;
    private void Awake()
    {
        if (ColorUtility.TryParseHtmlString("#FFFFFF", out Color color))
        {
            currentColor = color;
        }
    }
    public override void OnCountdownEnded()
    {
        if (isOnline)
        {
            if (PhotonHandler.Instance.IsHostAvailable(hostId))
            {
                int pixelOrder = UnityEngine.Random.Range(0, args.Count);
                foreach (var plId in memberIds)
                {
                    if (PhotonHandler.Instance.TryGetPlayerByActorNumber(plId, out var player))
                    {
                        PV.RPC(nameof(SetPixels), player, pixelOrder);
                        PV.RPC(nameof(Initialize), player);
                    }
                }
            }
        }
        else
        {
            int pixelOrder = UnityEngine.Random.Range(0, args.Count);
            SetPixels(pixelOrder);
            Initialize();
        }
    }
    [PunRPC]
    private void Initialize()
    {
        game.Play();
        clockTMP.enabled = true;
        currentTime = maxTime;
        clockTMP.text = maxTime.ToString();
        isInitialized = true;
        Utils.SetMouseLockedState(false);
    }
    public override void OnFinished(string winner)
    {
        isInitialized = false;
        winnerDisplay.ShowWinner(this, winner);
    }

    public override void ResetUI()
    {
        for (int i = 0; i < pixelBtns.Count; i++)
        {
            Destroy(pixelBtns[i].gameObject);
        }
        for (int i = 0; i < availableColorPrefabs.Count; i++)
        {
            Destroy(availableColorPrefabs[i].gameObject);
        }
        targetDrawingImage.sprite = null;
        pixelBtns.Clear();
        availableColors.Clear();
        availableColorPrefabs.Clear();
        isInitialized = false;
        clockTMP.enabled = false;
        Utils.SetMouseLockedState(true);
        targetDrawingImage.gameObject.SetActive(false);
    }

    public override void StartCountdown()
    {
        countdown.StartCountdown(this);
    }
    [PunRPC]
    private void SetPixels(int pixelOrder)
    {
        PixelDrawingArgs pixelArgs = args[pixelOrder];
        targetDrawingImage.gameObject.SetActive(true);
        targetDrawingImage.sprite = pixelArgs.pixelDrawing;
        PixelArgs targetPixelArgs = JsonHelper<PixelArgs>.Deserialize(pixelArgs.drawingArgsJson);
        float widthAndHeight = Mathf.Sqrt(targetPixelArgs.pixels.Count);
        pixelContainer.GetComponent<GridLayoutGroup>().cellSize = new Vector2(1000 / widthAndHeight, 1000 / widthAndHeight);
        //Debug.Log("target pixel count: " + targetPixelArgs.pixels.Count);
        currentPixelArgs.pixels = new();
        this.targetPixelArgs = targetPixelArgs;
        for (int i = 0; i < targetPixelArgs.pixels.Count; i++)
        {
            var pixel = targetPixelArgs.pixels.ElementAt(i);
            int pixelKey = pixel.Key;
            string pixelColorHTML = pixel.Value;
            if (!availableColors.Contains(pixelColorHTML))
            {
                Button availableColorBtn = Instantiate(availableColorPrefab, availableColorsContainer).GetComponent<Button>();
                availableColorBtn.gameObject.SetActive(true);
                Image availableColor = availableColorBtn.GetComponent<Image>();
                if (ColorUtility.TryParseHtmlString(pixelColorHTML, out Color color))
                {
                    availableColor.color = color;
                }
                availableColorBtn.onClick.AddListener(() => OnClick_AvailableColor(availableColor.color));
                availableColors.Add(pixelColorHTML);
                availableColorPrefabs.Add(availableColorBtn.transform);
            }
            currentPixelArgs.pixels.Add(pixelKey, "#FFFFFF");
            Button pixelBtn = Instantiate(pixelPrefab, pixelContainer).GetComponent<Button>();
            pixelBtn.gameObject.SetActive(true);
            pixelBtn.onClick.AddListener(() => OnClick_PixelButton(pixelKey, pixelBtn.GetComponent<Image>()));
            pixelBtns.Add(pixelBtn);
        }
    }
    private void OnClick_AvailableColor(Color color)
    {
        currentColor = color;
    }
    private void OnClick_PixelButton(int pixelOrder, Image pixel)
    {
        pixel.color = currentColor;
        string colorHex = "#" + ColorUtility.ToHtmlStringRGB(currentColor);
        currentPixelArgs.pixels[pixelOrder] = colorHex;
        CheckPixels();
    }
    private void CheckPixels()
    {
        for (int i = 0; i < currentPixelArgs.pixels.Count; i++)
        {
            if (currentPixelArgs.pixels.ElementAt(i).Value != targetPixelArgs.pixels.ElementAt(i).Value)
            {
                Debug.Log(currentPixelArgs.pixels.ElementAt(i).Value + " NOT MATCHED");
                return;
            }
        }
        game.Finish();
    }
}
