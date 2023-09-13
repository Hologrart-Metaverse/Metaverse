using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class LocalCameraHandler : MonoBehaviour
{
    [SerializeField] private Transform cameraAnchorPoint;
    [SerializeField] private Transform cinemachineFollowTarget;
    [SerializeField] private float cinemachineRotationSmoothValue;

    //Input
    Vector2 viewInput;

    //Rotation
    float cameraRotationX = 0;
    float cameraRotationY = 0;

    //Other components
    NetworkCharacterControllerPrototypeCustom networkCharacterControllerPrototypeCustom;
    Camera localCamera;
    private CinemachineVirtualCamera cinemachine;
    private void Awake()
    {
        localCamera = GetComponent<Camera>();
        networkCharacterControllerPrototypeCustom = GetComponentInParent<NetworkCharacterControllerPrototypeCustom>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //Detach camera if enabled
        if (localCamera.enabled)
            localCamera.transform.parent = null;
        if (NetworkPlayer.Local.HasInputAuthority)
        {
            GameInput.Instance.OnCameraChanged += GameInput_OnCameraChanged;
            cinemachine = FindObjectOfType<CinemachineVirtualCamera>();
            cinemachine.Follow = cinemachineFollowTarget;
        }
    }

    private void GameInput_OnCameraChanged(object sender, System.EventArgs e)
    {
        cinemachine.enabled = !cinemachine.enabled;
        cinemachine.transform.rotation = Quaternion.Euler(cameraRotationX, cameraRotationY, 0);
    }
    void LateUpdate()
    {
        if (cameraAnchorPoint == null)
            return;

        if (!localCamera.enabled)
            return;

        //Calculate rotation
        cameraRotationX += viewInput.y * Time.deltaTime * networkCharacterControllerPrototypeCustom.viewUpDownRotationSpeed;
        cameraRotationX = Mathf.Clamp(cameraRotationX, -90, 90);

        cameraRotationY += viewInput.x * Time.deltaTime * networkCharacterControllerPrototypeCustom.rotationSpeed;

        //Apply rotation
        if (!cinemachine.enabled)
        {
            //Move the camera to the position of the player
            localCamera.transform.position = cameraAnchorPoint.position;
            localCamera.transform.rotation = Quaternion.Euler(cameraRotationX, cameraRotationY, 0);
        }
        else
            cinemachine.transform.rotation = Quaternion.Lerp(cinemachine.transform.rotation,
                Quaternion.Euler(cameraRotationX, cameraRotationY, 0),
                Time.deltaTime * cinemachineRotationSmoothValue);
    }
    public void SetViewInputVector(Vector2 viewInput)
    {
        this.viewInput = viewInput;
    }
}
