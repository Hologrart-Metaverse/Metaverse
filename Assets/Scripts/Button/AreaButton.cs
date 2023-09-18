using TMPro;
using UnityEngine;

public class AreaButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI areaTMP;

    public void InitializeButton(string areaText)
    {
        areaTMP.text = areaText;
    }
}
