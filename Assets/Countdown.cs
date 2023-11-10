using TMPro;
using UnityEngine;
using DG.Tweening;
using System.Collections;

public class Countdown : MonoBehaviour
{
    [SerializeField] private int startValue;
    [SerializeField] private TextMeshProUGUI countdownTMP;
    private int currentValue;
    private GameUI gameUI;
    public void StartCountdown(GameUI _gameUI)
    {
        gameUI = _gameUI;
        currentValue = startValue;
        countdownTMP.enabled = true;
        countdownTMP.text = currentValue.ToString();
        countdownTMP.transform.localScale = Vector3.zero;
        StartCoroutine(CountdownNumerator());
    }
    private IEnumerator CountdownNumerator()
    {
        while (currentValue > 0)
        {
            countdownTMP.transform.DOScale(Vector3.one, .3f);
            yield return new WaitForSeconds(1);
            currentValue -= 1;
            countdownTMP.text = currentValue.ToString();
            countdownTMP.transform.localScale = Vector3.zero;
        }
        countdownTMP.text = "START";
        countdownTMP.transform.DOScale(Vector3.one, .3f);
        yield return new WaitForSeconds(1);
        countdownTMP.enabled = false;
        gameUI.OnCountdownEnded();
    }
}
