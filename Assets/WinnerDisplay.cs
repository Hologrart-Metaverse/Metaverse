using System.Collections;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class WinnerDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI winnerTMP;
    private GameUI gameUI;
    public void ShowWinner(GameUI _gameUI, string winnerName)
    {
        gameUI = _gameUI;
        winnerTMP.enabled = true;
        winnerTMP.transform.localScale = Vector3.zero;
        StartCoroutine(WinnerNumerator(winnerName));
    }
    private IEnumerator WinnerNumerator(string winner)
    {
        winnerTMP.text = "WINNER IS\n" + winner;
        winnerTMP.transform.DOScale(Vector3.one, 1f);
        yield return new WaitForSeconds(2.5f);
        winnerTMP.enabled = false;
        gameUI.OnWinnerShowed();
    }
}
