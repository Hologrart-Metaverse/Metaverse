using TMPro;
using UnityEngine;

public class UI_EscapeFromMaze : GameUI
{
    [SerializeField] private TextMeshProUGUI clockTMP;
    [SerializeField] private float maxTime;

    public override void Initialize()
    {
        clockTMP.text = maxTime.ToString();
    }
    private void FixedUpdate()
    {
        if (isInitialized)
        {
            maxTime -= Time.fixedDeltaTime;
            clockTMP.text = Mathf.FloorToInt(maxTime).ToString();
        }
    }
}
