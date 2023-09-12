using System;
using UnityEngine;
using UnityEngine.InputSystem;
public class GameInput : MonoBehaviour
{
    public static GameInput Instance { get; private set; }

    private InputSystem inputActions;

    public event EventHandler OnInteractAction;
    public event EventHandler OnChangeCam;
    public event EventHandler OnEnterPressed;
    //public event EventHandler OnAttack;

    private void Awake()
    {
        if (!Instance)
            Instance = this;

        inputActions = new InputSystem();

        inputActions.Player.Enable();

        inputActions.Player.Interact.performed += Interaction_performed;
        inputActions.Player.ChangeCam.performed += ChangeCam_performed;
        inputActions.Player.Enter.performed += Enter_performed;
        //inputActions.Player.Fire.performed += Fire_performed;
    }

    private void Enter_performed(InputAction.CallbackContext obj)
    {
        OnEnterPressed?.Invoke(this, EventArgs.Empty);
    }


    //private void Fire_performed(InputAction.CallbackContext obj)
    //{
    //    OnAttack?.Invoke(this, EventArgs.Empty);
    //}

    private void ChangeCam_performed(InputAction.CallbackContext obj)
    {
        OnChangeCam?.Invoke(this, EventArgs.Empty);
    }

    private void Interaction_performed(InputAction.CallbackContext obj)
    {
        OnInteractAction?.Invoke(this, EventArgs.Empty);
    }

    private void OnDestroy()
    {
        inputActions.Player.Interact.performed -= Interaction_performed;
        inputActions.Player.ChangeCam.performed -= ChangeCam_performed;
        inputActions.Dispose();
    }
    public InputSystem GetInputActions()
    {
        return inputActions;
    }
    public bool IsJumpButtonPressed()
    {
        return inputActions.Player.Jump.IsPressed();
    }
    public Vector2 GetMovementVectorNormalized()
    {
        Vector2 inputVector = inputActions.Player.Move.ReadValue<Vector2>();

        inputVector = inputVector.normalized;

        return inputVector;
    }
    public Vector2 GetMouseLook()
    {
        Vector2 lookVector = inputActions.Player.Look.ReadValue<Vector2>();
        return lookVector;
    }
}