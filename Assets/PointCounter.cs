using TMPro;
using UnityEngine;

public class PointCounter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI pointTMP;
    private float currentPoint;
    private void OnEnable()
    {
        currentPoint = 0;
        pointTMP.text = currentPoint.ToString();
    }

    public void AddPoint(float point)
    {
        currentPoint += point;
        pointTMP.text = currentPoint.ToString();
        if(currentPoint >= 50)
        {
            GetComponentInParent<Game>().Finish();
        }
    } 
}
