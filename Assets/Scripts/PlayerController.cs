using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharacterController controller;
    private Transform cam;
    private Transform groundCheck;
    private LayerMask groundMask;

    private float speed = 5f;
    // private float mouseSensitivity = 100f;
    private float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity = 0.1f;

    private float jumpHeight = 1f;
    private float groundDistance = 0.2f;
    private bool isGrounded;

    private float gravity = -9.81f;
    Vector3 velocity;
    
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // Locks the cursor to the center of the screen
        controller = gameObject.GetComponent<CharacterController>(); // Gets the character controller component
        cam = GameObject.Find("Main Camera").transform; // Gets the main camera
        groundCheck = GameObject.Find("GroundCheck").transform; // Gets the ground check
        groundMask = LayerMask.GetMask("Ground"); // Gets the ground layer
    }

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask); // Checks if the player is on the ground

        float x = Input.GetAxis("Horizontal"); // Gets the horizontal input
        float z = Input.GetAxis("Vertical"); // Gets the vertical input

        // GRAVITY
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
        
        // JUMPING
        if (Input.GetButtonDown("Jump") && isGrounded) // If the player presses the jump button and is on the ground
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity); // Sets the velocity to the square root of the jump height times -2 times gravity
        }

        // MOVEMENT
        Vector3 direction = new Vector3(x, 0f, z).normalized;
        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * speed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.LeftShift)) // If the player presses the left shift key
        {
            speed = 5f;
        }
        else 
        {
            speed = 2f;
        }
    }
}
