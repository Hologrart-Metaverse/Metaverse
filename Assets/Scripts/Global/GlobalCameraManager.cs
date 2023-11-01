using Cinemachine;
using UnityEngine;

public class GlobalCameraManager : MonoBehaviour
{
    private Camera playerCam;
    private LayerMask defaultLayerMask;
    [SerializeField] private LayerMask playerHidedLayerMask;
    private CinemachineVirtualCamera currentVirtualCamera;
    private float currentFOV;
    public enum CameraType
    {
        Main,
        Details,
    }
    public static GlobalCameraManager Instance;

    [SerializeField] private CinemachineVirtualCamera detailsCM;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        StateHandler.Instance.StateChanged += StateHandler_StateChanged;
        Spawner.Instance.OnPlayerSpawned += Spawner_OnPlayerSpawned;
    }

    private void Spawner_OnPlayerSpawned(object sender, System.EventArgs e)
    {
        playerCam = PlayerController.Local.GetCamera();
        defaultLayerMask = playerCam.cullingMask;
        Spawner.Instance.OnPlayerSpawned -= Spawner_OnPlayerSpawned;
    }

    private void StateHandler_StateChanged(StateHandler sender, State state)
    {
        if (playerCam is null)
            return;

        switch (state)
        {
            case State.None:
                playerCam.cullingMask = defaultLayerMask;
                ResetCurrentVirtualCamera();
                break;
            default:
                playerCam.cullingMask = playerHidedLayerMask;
                break;
        }
    }
    public CinemachineVirtualCamera GetVirtualCamera(CameraType cameraType)
    {
        switch (cameraType)
        {
            default:
            case CameraType.Details:
                SetCurrentVirtualCamera(detailsCM);
                return detailsCM;
        }
    }
    private void SetCurrentVirtualCamera(CinemachineVirtualCamera virtualCamera)
    {
        currentVirtualCamera = virtualCamera;
        currentFOV = currentVirtualCamera.m_Lens.FieldOfView;
    }
    private void ResetCurrentVirtualCamera()
    {
        currentVirtualCamera.m_Lens.FieldOfView = currentFOV;
    }
}
