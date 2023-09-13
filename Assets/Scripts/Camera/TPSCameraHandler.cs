using Cinemachine;
using UnityEngine;

public class TPSCameraHandler : MonoBehaviour
{
    public static TPSCameraHandler Instance;
    private CinemachineVirtualCamera cinemachine;
    [SerializeField] private GameObject cam;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        cinemachine = GetComponent<CinemachineVirtualCamera>();
    }

}
