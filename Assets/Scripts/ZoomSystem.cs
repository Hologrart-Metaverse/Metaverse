using Cinemachine;
using UnityEngine;
using ActionCode.Attributes;
using UnityEngine.InputSystem;
public class ZoomSystem : MonoBehaviour
{
    public static ZoomSystem Instance;
    public enum ZoomMode
    {
        Dynamic,
        Static,
    }
    public enum ZoomSurface
    {
        JointNft,
        Artifact
    }
    private CinemachineVirtualCamera detailCam;
    private CinemachineFramingTransposer transposer;
    private ZoomMode currentZoomMode;
    internal ZoomSurface currentZoomSurface;
    private float camFOVSizeMin = 10f;
    private float camFOVSizeMax = 40f;
    private float camClampValue = 2.2f;
    private Transform objectTransform;
    private float speed = 0.08f;

    private float prevMagnitude = 0;
    private int touchCount = 0;
    private bool isScrollPressed;
    private Vector3 camInitPos;

    private Transform objectFollowTransform;
    private Vector3 objectInitialPosition;
    internal bool isZooming;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        detailCam = GlobalCameraManager.Instance.GetVirtualCamera(GlobalCameraManager.CameraType.Details);
        transposer = detailCam.GetCinemachineComponent<CinemachineFramingTransposer>();

        Spawner.Instance.OnPlayerSpawned += Spawner_OnPlayerSpawned;
        camInitPos = transposer.m_TrackedObjectOffset;
        // mouse scroll
        var scrollAction = new InputAction(binding: "<Mouse>/scroll");
        scrollAction.Enable();
        scrollAction.performed += ctx => CameraZoom(ctx.ReadValue<Vector2>().y * speed);

        // pinch gesture
        var touch0contact = new InputAction
        (
            type: InputActionType.Button,
            binding: "<Touchscreen>/touch0/press"
        );
        touch0contact.Enable();
        var touch1contact = new InputAction
        (
            type: InputActionType.Button,
            binding: "<Touchscreen>/touch1/press"
        );
        touch1contact.Enable();

        touch0contact.performed += _ => touchCount++;
        touch1contact.performed += _ => touchCount++;
        touch0contact.canceled += _ =>
        {
            touchCount--;
            prevMagnitude = 0;
        };
        touch1contact.canceled += _ =>
        {
            touchCount--;
            prevMagnitude = 0;
        };

        var touch0pos = new InputAction
        (
            type: InputActionType.Value,
            binding: "<Touchscreen>/touch0/position"
        );
        touch0pos.Enable();
        var touch1pos = new InputAction
        (
            type: InputActionType.Value,
            binding: "<Touchscreen>/touch1/position"
        );
        touch1pos.Enable();
        touch1pos.performed += _ =>
        {
            if (touchCount < 2)
                return;
            var magnitude = (touch0pos.ReadValue<Vector2>() - touch1pos.ReadValue<Vector2>()).magnitude;
            if (prevMagnitude == 0)
                prevMagnitude = magnitude;
            var difference = magnitude - prevMagnitude;
            prevMagnitude = magnitude;
            CameraZoom(-difference * speed);
        };

        GameInput.Instance.GetInputActions().Player.ScrollPress.started += ScrollButtonPressed;
        GameInput.Instance.GetInputActions().Player.ScrollPress.canceled += ScrollButtonReleased;
    }

    private void Spawner_OnPlayerSpawned(object sender, System.EventArgs e)
    {
        objectFollowTransform = PlayerController.Local.GetItemExaminationPoint();
        Spawner.Instance.OnPlayerSpawned -= Spawner_OnPlayerSpawned;
    }

    public void StartZooming(ZoomMode zoomMode = ZoomMode.Static, ZoomSurface zoomSurface = ZoomSurface.Artifact, Transform zoomObject = null)
    {
        currentZoomMode = zoomMode;
        currentZoomSurface = zoomSurface;
        switch (currentZoomMode)
        {
            case ZoomMode.Static:
                SetZoomAdjustments(zoomSurface);
                transposer.m_TrackedObjectOffset = Vector3.zero;
                detailCam.m_Lens.FieldOfView = camFOVSizeMax;
                break;
            case ZoomMode.Dynamic:
                objectTransform = zoomObject.GetComponentInParent<Artifact>().transform;
                objectInitialPosition = objectTransform.position;
                break;
        }
        isZooming = true;
    }
    private void SetZoomAdjustments(ZoomSurface zoomSurface)
    {
        switch (zoomSurface)
        {
            case ZoomSurface.Artifact:
                camFOVSizeMin = 20f;
                camFOVSizeMax = 40f;
                break;
            case ZoomSurface.JointNft:
                camFOVSizeMin = 60f;
                camFOVSizeMax = 120f;
                break;
        }
        camClampValue = camFOVSizeMin / 40f;
        speed = 0.05f + camClampValue / 20f;
    }
    public void EndZooming()
    {
        isZooming = false;
        if (currentZoomMode == ZoomMode.Dynamic)
        {
            objectTransform.position = objectInitialPosition;
            objectTransform.rotation = Quaternion.identity;
        }
    }
    private void ScrollButtonPressed(InputAction.CallbackContext obj)
    {
        isScrollPressed = true;
    }
    private void ScrollButtonReleased(InputAction.CallbackContext obj)
    {
        isScrollPressed = false;
    }
    private void Update()
    {
        if (isScrollPressed && isZooming)
        {
            if (currentZoomMode == ZoomMode.Static && detailCam.m_Lens.FieldOfView < camFOVSizeMax)
            {
                Vector3 cameraMovement = new Vector3(GameInput.Instance.GetMouseLook().x, GameInput.Instance.GetMouseLook().y, 0) * speed;
                if (currentZoomSurface == ZoomSurface.Artifact) cameraMovement.x *= -1;
                Vector3 predictedPosition = transposer.m_TrackedObjectOffset + cameraMovement;
                if (predictedPosition.y < camInitPos.y + camClampValue &&
                   predictedPosition.y > camInitPos.y - camClampValue &&
                   predictedPosition.x < camInitPos.x + camClampValue &&
                   predictedPosition.x > camInitPos.x - camClampValue)
                {
                    transposer.m_TrackedObjectOffset += cameraMovement;
                }
            }
            else if (currentZoomMode == ZoomMode.Dynamic)
            {
                Vector3 cameraMovement = new Vector3(GameInput.Instance.GetMouseLook().y, GameInput.Instance.GetMouseLook().x, GameInput.Instance.GetMouseLook().y);
                objectTransform.Rotate(cameraMovement);
            }
        }
    }
    private void LateUpdate()
    {
        if (currentZoomMode == ZoomMode.Dynamic && isZooming)
        {
            objectTransform.transform.localPosition = Vector3.Lerp(objectTransform.transform.localPosition, objectFollowTransform.position, Time.deltaTime * 5f);
        }
    }
    private void CameraZoom(float increment)
    {
        if (currentZoomMode == ZoomMode.Static && isZooming)
        {
            detailCam.m_Lens.FieldOfView = Mathf.Clamp(detailCam.m_Lens.FieldOfView - increment * speed, camFOVSizeMin, camFOVSizeMax);
            if (detailCam.m_Lens.FieldOfView >= camFOVSizeMax)
            {
                transposer.m_TrackedObjectOffset = camInitPos;
            }
        }
    }

    public void ResetView()
    {
        transposer.m_TrackedObjectOffset = camInitPos;
        detailCam.m_Lens.FieldOfView = camFOVSizeMax;
    }
}
