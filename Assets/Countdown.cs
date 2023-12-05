using TMPro;
using UnityEngine;
using DG.Tweening;
using System.Collections;

public class Countdown : MonoBehaviour
{
    [SerializeField] private int startValue;
    [SerializeField] private TextMeshProUGUI countdownTMP;
    [SerializeField] private Transform backgroundTransform;
    [SerializeField] private Transform topCountdownTransform;
    [SerializeField] private Transform UI;
    private int currentValue;
    private GameUI gameUI;
    private float currentBackgroundScale;
    public void StartCountdown(GameUI _gameUI)
    {
        Utils.SetParent(backgroundTransform, UI, true);
        currentBackgroundScale = 1f;
        backgroundTransform.localScale = Utils.BuildVectorWithSameFloat(currentBackgroundScale);
        backgroundTransform.localPosition = Vector3.zero;
        gameUI = _gameUI;
        currentValue = startValue;
        countdownTMP.enabled = true;
        countdownTMP.text = currentValue.ToString();
        StartCoroutine(CountdownNumerator());
    }
    private IEnumerator CountdownNumerator()
    {
        while (currentValue > 0)
        {
            countdownTMP.transform.DOLocalRotate(new Vector3(0, 0, 360), .3f, RotateMode.FastBeyond360).SetRelative(true).SetEase(Ease.Linear);
            yield return new WaitForSecondsRealtime(1);
            currentBackgroundScale += 3f;
            backgroundTransform.DOScale(Utils.BuildVectorWithSameFloat(currentBackgroundScale), .75f).SetEase(Ease.OutBack);
            currentValue -= 1;
            countdownTMP.text = currentValue.ToString();
        }
        countdownTMP.text = "START";
        yield return new WaitForSecondsRealtime(1);
        Utils.SetParent(backgroundTransform, topCountdownTransform, true);
        backgroundTransform.DOScale(Vector3.one, .3f);
        backgroundTransform.DOLocalMove(Vector3.zero, .3f).OnComplete(() => Utils.SetParent(backgroundTransform, UI, true));
        countdownTMP.enabled = false;
        gameUI.OnCountdownEnded();
    }
}
