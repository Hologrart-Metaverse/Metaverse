using UnityEngine;
using UnityEngine.EventSystems;

public class CharacterInputHandler : MonoBehaviour
{
    private bool isJumpButtonPressed;

    private LocalCameraHandler _localCameraHandler;
    private void Awake()
    {
        _localCameraHandler = GetComponentInChildren<LocalCameraHandler>();
    }
    private void Start()
    {
        Utils.SetMouseLockedState(true);
    }
    private void Update()
    {
        //Jump
        if (GameInput.Instance.IsJumpButtonPressed())
            isJumpButtonPressed = true;

        //Set view
        _localCameraHandler.SetViewInputVector(GameInput.Instance.GetMouseLook());
    }
    public NetworkInputData GetNetworkInput()
    {
        NetworkInputData networkInputData = new NetworkInputData();

        if(EventSystem.current.currentSelectedGameObject == null)
        {
            networkInputData.movementInput = GameInput.Instance.GetMovementVectorNormalized();

            networkInputData.aimForwardVector = _localCameraHandler.transform.forward;

            networkInputData.isJumpPressed = isJumpButtonPressed;
        }
        else
        {
            networkInputData.movementInput = Vector2.zero;
            networkInputData.aimForwardVector = Vector2.zero;
        }
        isJumpButtonPressed = false;

        return networkInputData;
    }
}
