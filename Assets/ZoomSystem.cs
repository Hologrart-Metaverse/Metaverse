using UnityEngine;
using UnityEngine.InputSystem;
public class ZoomSystem : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private float camOrthographicSizeMin = .5f;
    [SerializeField] private float camOrthographicSizeMax = 3.3f;
    public float speed = 0.01f;
    private float prevMagnitude = 0;
    private int touchCount = 0;
    private bool isScrollPressed;
    private Vector3 camInitPos;
    private float camZPosClampValue = 2.2f;
    private float camYPosClampValue = 2.2f;
    private void Awake()
    {
        camInitPos = cam.transform.position;
        Debug.Log(camInitPos);
    }
    private void Start()
    {
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
        if (isScrollPressed && cam.orthographicSize < camOrthographicSizeMax)
        {
            Vector3 cameraMovement = new Vector3(0, GameInput.Instance.GetMouseLook().y, -GameInput.Instance.GetMouseLook().x) * Time.fixedDeltaTime;
            Vector3 predictedPosition = cam.transform.position + cameraMovement;
            if (predictedPosition.y < camInitPos.y + camYPosClampValue &&
               predictedPosition.y > camInitPos.y - camYPosClampValue &&
               predictedPosition.z < camInitPos.z + camZPosClampValue &&
               predictedPosition.z > camInitPos.z - camZPosClampValue)
            {
                cam.transform.position += cameraMovement;
            }
        }
    }
    private void CameraZoom(float increment)
    {
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize - increment * Time.fixedDeltaTime, camOrthographicSizeMin, camOrthographicSizeMax);
        if (cam.orthographicSize >= camOrthographicSizeMax)
        {
            cam.transform.position = camInitPos;
        }
    }

    public void ResetView()
    {
        cam.transform.position = camInitPos;
        cam.orthographicSize = camOrthographicSizeMax;
    }
}
