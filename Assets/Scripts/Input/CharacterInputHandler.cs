using UnityEngine;
using UnityEngine.EventSystems;

public class CharacterInputHandler : MonoBehaviour
{
    private bool isJumpButtonPressed;
    private bool isRunning;
    private LocalCameraHandler _fpsCameraHandler;
    private void Awake()
    {
        _fpsCameraHandler = GetComponentInChildren<LocalCameraHandler>();
    }
    private void Start()
    {
        Utils.SetMouseLockedState(true);
    }
    private void Update()
    {
        if (EventSystem.current.currentSelectedGameObject != null)
            return;

        //Jump
        if (GameInput.Instance.IsJumpButtonPressed())
            isJumpButtonPressed = true;
        //Sprint
        if(GameInput.Instance.IsRunning())
            isRunning = true;
        //Set view
        _fpsCameraHandler.SetViewInputVector(GameInput.Instance.GetMouseLook());
    }
    public NetworkInputData GetNetworkInput()
    {
        NetworkInputData networkInputData = new NetworkInputData();

        if (EventSystem.current.currentSelectedGameObject == null)
        {
            networkInputData.movementInput = GameInput.Instance.GetMovementVectorNormalized();

            networkInputData.aimForwardVector = _fpsCameraHandler.transform.forward;

            networkInputData.isJumpPressed = isJumpButtonPressed;

            networkInputData.isRunning = isRunning;
        }

        isJumpButtonPressed = false;
        isRunning = false;

        return networkInputData;
    }
}
