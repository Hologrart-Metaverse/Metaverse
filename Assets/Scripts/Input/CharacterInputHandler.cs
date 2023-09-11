using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInputHandler : MonoBehaviour
{
    private Vector2 moveInputVector = Vector2.zero;
    private Vector2 viewInputVector = Vector2.zero;
    private bool isJumpButtonPressed;

    private CharacterMovementHandler _characterMovementHandler;
    private void Awake()
    {
        _characterMovementHandler = GetComponent<CharacterMovementHandler>();
    }
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void Update()
    {
        //View Input
        viewInputVector.x = Input.GetAxis("Mouse X");
        viewInputVector.y = Input.GetAxis("Mouse Y") * -1; //Invert the mouse look

        _characterMovementHandler.SetViewInputVector(viewInputVector);
        //Move Input
        moveInputVector.x = Input.GetAxis("Horizontal");
        moveInputVector.y = Input.GetAxis("Vertical");

        //Jump
        isJumpButtonPressed = Input.GetButtonDown("Jump");
    }
    public NetworkInputData GetNetworkInput()
    {
        NetworkInputData networkInputData = new NetworkInputData();

        networkInputData.movementInput = moveInputVector;
        networkInputData.rotationInput = viewInputVector.x;
        networkInputData.isJumpPressed = isJumpButtonPressed;
        return networkInputData;
    }
}
