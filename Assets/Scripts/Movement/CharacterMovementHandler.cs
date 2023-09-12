using Fusion;
using UnityEngine;

public class CharacterMovementHandler : NetworkBehaviour
{
    private NetworkCharacterControllerPrototypeCustom _networkCharacterController;
    private void Awake()
    {
        _networkCharacterController = GetComponent<NetworkCharacterControllerPrototypeCustom>();
    }
    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData input))
        {
            //Rotate
            transform.forward = input.aimForwardVector;

            //Fix tilt
            Quaternion rot = transform.rotation;
            rot.eulerAngles = new Vector3(0, rot.eulerAngles.y, rot.eulerAngles.z);
            transform.rotation = rot;

            //Move
            Vector3 moveDirection = transform.forward * input.movementInput.y + transform.right * input.movementInput.x;
            moveDirection.Normalize();

            _networkCharacterController.Move(moveDirection);

            //Jump
            if (input.isJumpPressed)
            {
                _networkCharacterController.Jump();
            }

            CheckFallRespawn();
        }
    }
    private void CheckFallRespawn()
    {
        if (transform.position.y < -12)
        {
            transform.position = Utils.GetRandomPosition();
        }
    }
  
}
