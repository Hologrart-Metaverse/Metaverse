using TMPro;
using UnityEngine;

public class ArtifactDetailsUI : MonoBehaviour
{
    public static ArtifactDetailsUI Instance { get; private set; }

    [SerializeField] private Transform contentTransform;
    [SerializeField] private TextMeshProUGUI artifactNameTMP;
    [SerializeField] private TextMeshProUGUI detailsTMP;
    private bool isShowed;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        contentTransform.gameObject.SetActive(false);
    }
    public void Show(string artifactName, string details)
    {
        isShowed = true;
        contentTransform.gameObject.SetActive(true);
        artifactNameTMP.text = artifactName;
        detailsTMP.text = details;
    }
    public void Hide()
    {
        isShowed = false;
        contentTransform.gameObject.SetActive(false);
    }
}
