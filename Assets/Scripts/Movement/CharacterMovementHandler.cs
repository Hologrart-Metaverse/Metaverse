using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovementHandler : NetworkBehaviour
{
    private NetworkCharacterControllerPrototypeCustom _networkCharacterController;
    private Vector2 viewInput;
    private float cameraRotationX = 0;
    private Camera localCamera;
    private void Awake()
    {
        _networkCharacterController = GetComponent<NetworkCharacterControllerPrototypeCustom>();
        localCamera = GetComponentInChildren<Camera>();
    }
    private void Update()
    {
        cameraRotationX += viewInput.y * Time.deltaTime * _networkCharacterController.viewUpAndDownRotationSpeed;
        cameraRotationX = Mathf.Clamp(cameraRotationX, -90, 90);
        localCamera.transform.localRotation = Quaternion.Euler(cameraRotationX, 0, 0);
    }
    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData input))
        {
            //Rotate
            _networkCharacterController.Rotate(input.rotationInput);

            //Move
            Vector3 moveDirection = transform.forward * input.movementInput.y + transform.right * input.movementInput.x;
            moveDirection.Normalize();

            _networkCharacterController.Move(moveDirection);

            //Jump
            if (input.isJumpPressed)
            {
                _networkCharacterController.Jump();
            }
        }
    }

    public void SetViewInputVector(Vector2 viewInput)
    {
        this.viewInput = viewInput;
    }
}
