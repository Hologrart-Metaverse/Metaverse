using Cinemachine;
using Photon.Pun;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerController : MonoBehaviourPunCallbacks
{
    public static PlayerController Local { get; private set; }

    public event EventHandler OnPlayerDie;
    public event EventHandler OnPlayerSpawned;
    [SerializeField] private CharacterController Controller;
    [SerializeField] private Camera cam;
    [SerializeField] private CinemachineVirtualCamera fpsCam;
    [SerializeField] private CinemachineVirtualCamera tpsCam;
    [SerializeField] private CinemachineBrain cinemachineBrain;
    [SerializeField] private GameObject CinemachineCameraTarget;
    [SerializeField] internal float sprintSpeed, walkSpeed, smoothTime;
    [SerializeField] private Animator _animator;
    [SerializeField] private Transform itemExaminationPoint; 
    public float mouseSensitivity;
    private bool grounded;
    private Vector3 smoothMoveVelocity;
    private Vector3 moveAmount;
    private PhotonView PV;
    [SerializeField] internal GameObject player;
    //interactions
    private bool interacted;
    private I_Interactable lastInteracted;
    private bool facedAlready;

    private Vector3 stopPl = Vector3.zero;
    //tps cam rotation
    private float _cinemachineTargetYaw = 0;
    private float _cinemachineTargetPitch = 0;
    private float TopClamp = 70.0f;
    private float BottomClamp = -30.0f;
    private float CameraAngleOverride = 0.0f;

    // animation IDs
    private int _animIDSpeed;
    private int _animIDGrounded;
    private int _animIDJump;
    private int _animIDFreeFall;
    private int _animIDMotionSpeed;

    private float _speed;
    [SerializeField] private float _speedChangingSmoothness = 5f;

    public AudioClip LandingAudioClip;
    public AudioClip[] FootstepAudioClips;
    [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

    [Space(10)]
    [Tooltip("The height the player can jump")]
    public float JumpHeight = 1.2f;

    [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
    public float Gravity = -15.0f;

    [Space(10)]
    [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
    public float JumpTimeout = 0.50f;

    [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
    public float FallTimeout = 0.4f;

    [Tooltip("Useful for rough ground")]
    public float GroundedOffset = -0.14f;

    [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
    public float GroundedRadius = 0.28f;

    [Tooltip("What layers the character uses as ground")]
    public LayerMask GroundLayers;

    private float _verticalVelocity;
    private float _terminalVelocity = 53.0f;

    // timeout deltatime
    private float _jumpTimeoutDelta;
    private float _fallTimeoutDelta;
    private float coyoteTime;
    private float coyoteTimeMax = .15f;
    private bool checkCoyoteOnce = true;

    private Ray ray;
    void Awake()
    {
        PV = GetComponent<PhotonView>();
        if (PV.IsMine && Local == null)
            Local = this;
    }

    void Start()
    {
        if (PV.IsMine)
        {
            _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;
            OnPlayerSpawned?.Invoke(this, EventArgs.Empty);
            AssignAnimationIDs();
            Spawner.Instance.PlayerSpawned();
        }
        else
        {
            Destroy(cam.gameObject);
            Destroy(fpsCam.gameObject);
            Destroy(tpsCam.gameObject);
        }
    }
    private void Update()
    {
        if (!PV.IsMine || !StateHandler.Instance.IsMovable())
        {
            return;
        }
        CamCheck();
        JumpAndGravity();
        CheckGroundedState();
        HandleInteractions();
    }
    private void FixedUpdate()
    {
        if (!PV.IsMine)
        {
            return;
        }
        if (!StateHandler.Instance.IsMovable())
        {
            Move(false);
            return;
        }
        else
            cinemachineBrain.enabled = true;
        Move();
    }
    private void LateUpdate()
    {
        if (!PV.IsMine || !StateHandler.Instance.IsMovable())
            return;
        CameraRotation();
    }
    private void OnDestroy()
    {
        OnPlayerDie?.Invoke(this, EventArgs.Empty);
    }
 
    private void AssignAnimationIDs()
    {
        _animIDSpeed = Animator.StringToHash("Speed");
        _animIDGrounded = Animator.StringToHash("Grounded");
        _animIDJump = Animator.StringToHash("Jump");
        _animIDFreeFall = Animator.StringToHash("FreeFall");
        _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
    }
    public Animator GetAnimator()
    {
        return _animator;
    }
    public Camera GetCamera()
    {
        return cam;
    }
    public Transform GetPlayer()
    {
        return player.transform;
    }
    private void HandleInteractions()
    {
        ray = cam.GetComponent<Camera>().ViewportPointToRay(new Vector3(0.5f, 0.5f));
        ray.origin = cam.transform.position;
        float interactDistance = fpsCam.isActiveAndEnabled ? 3.5f : 7;
        if (Physics.Raycast(ray, out RaycastHit raycastHit, interactDistance))
        {
            if (raycastHit.transform.TryGetComponent(out I_Interactable interactableObj))
            {
                //interactable object has found
                interacted = true;
                lastInteracted = interactableObj;
                if (!facedAlready)
                {
                    interactableObj.OnFaced();
                    facedAlready = true;
                }
                if (GameInput.Instance.GetInputActions().Player.Interact.triggered)
                {
                    interactableObj.Interact();
                }
            }
            else
            {
                if (interacted && lastInteracted != null)
                {
                    lastInteracted.OnInteractEnded();
                    interacted = false;
                    lastInteracted = null;
                }
                facedAlready = false;
            }
        }
        else
        {
            if (interacted && lastInteracted != null)
            {
                lastInteracted.OnInteractEnded();
                interacted = false;
                lastInteracted = null;
            }
            facedAlready = false;
        }

    }

    private void CamCheck()
    {
        if (EventSystem.current.currentSelectedGameObject)
            return;

        else if (GameInput.Instance.GetInputActions().Player.ChangeCam.triggered)
        {
            fpsCam.enabled = !fpsCam.isActiveAndEnabled;
            tpsCam.enabled = !tpsCam.isActiveAndEnabled;
            int layer = fpsCam.enabled ? 6 : 7;
            Utils.SetRenderLayerInChildren(player.transform, layer);
        }

    }

    void Move(bool isMovable = true)
    {
        if (EventSystem.current.currentSelectedGameObject || !isMovable)
        {
            moveAmount = stopPl;
            _animator.SetFloat("Speed", 0);
            return;
        }

        Vector2 movement = GameInput.Instance.GetMovementVectorNormalized();


        Vector3 forward = cam.transform.forward;
        Vector3 right = cam.transform.right;

        forward.y = 0f; // Y ekseni üzerindeki dönüþü önlemek için sýfýrlayýn.
        right.y = 0f;

        Vector3 moveDir = (forward * movement.y + right * movement.x).normalized;

        // Yerçekimi etkisi olmasýn
        moveDir.y = 0f;

        if (GameInput.Instance.GetInputActions().Player.Sprint.IsPressed())
        {
            moveAmount = Vector3.SmoothDamp(moveAmount, moveDir * sprintSpeed, ref smoothMoveVelocity, smoothTime);
        }
        else
        {
            moveAmount = Vector3.SmoothDamp(moveAmount, moveDir * walkSpeed, ref smoothMoveVelocity, smoothTime);
        }
        _speed = Mathf.Lerp(_speed, moveAmount.magnitude, Time.fixedDeltaTime * _speedChangingSmoothness);
        _animator.SetFloat("Speed", _speed);
        //Rotation
        if (tpsCam.isActiveAndEnabled)
        {
            if (movement != Vector2.zero)
            {
                float targetAngle = Mathf.Atan2(movement.x, movement.y) * Mathf.Rad2Deg + cam.transform.eulerAngles.y;
                Quaternion rotation = Quaternion.Euler(0f, targetAngle, 0f);
                player.transform.rotation = Quaternion.Lerp(player.transform.rotation, rotation, Time.deltaTime * 4f);
            }
        }
        else if (fpsCam.isActiveAndEnabled)
        {
            Quaternion rotation = Quaternion.Euler(0f, cinemachineBrain.transform.eulerAngles.y, 0f);
            player.transform.rotation = Quaternion.Lerp(player.transform.rotation, rotation, Time.deltaTime * 4f);
        }
        Controller.Move(transform.TransformDirection(moveAmount) * Time.fixedDeltaTime +
            new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.fixedDeltaTime);

        CheckFallRespawn();
    }
    private void JumpAndGravity()
    {
        if (grounded)
        {
            // reset the fall timeout timer
            _fallTimeoutDelta = FallTimeout;

            // stop our velocity dropping infinitely when grounded
            if (_verticalVelocity < 0.0f)
            {
                _verticalVelocity = -2f;
            }

            // Jump
            if (GameInput.Instance.IsJumpButtonPressed() && _jumpTimeoutDelta <= 0.0f && !EventSystem.current.currentSelectedGameObject)
            {
                // the square root of H * -2 * G = how much velocity needed to reach desired height
                _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

                _animator.SetBool(_animIDJump, true);
            }

            // jump timeout
            if (_jumpTimeoutDelta >= 0.0f)
            {
                _jumpTimeoutDelta -= Time.deltaTime;
            }
        }
        else
        {
            // reset the jump timeout timer
            _jumpTimeoutDelta = JumpTimeout;

            // fall timeout
            if (_fallTimeoutDelta >= 0.0f)
            {
                _fallTimeoutDelta -= Time.deltaTime;
            }
            else
            {

                _animator.SetBool(_animIDFreeFall, true);
            }

            // if we are not grounded, do not jump
            //_input.jump = false;
        }

        // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
        if (_verticalVelocity < _terminalVelocity)
        {
            _verticalVelocity += Gravity * Time.deltaTime;
        }
    }
    private void CameraRotation()
    {
        _cinemachineTargetYaw += GameInput.Instance.GetMouseLook().x * mouseSensitivity;
        _cinemachineTargetPitch += GameInput.Instance.GetMouseLook().y * mouseSensitivity;
        // clamp our rotations so our values are limited 360 degrees
        _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);
        CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
            _cinemachineTargetYaw, Time.deltaTime * 4f);

    }
    private void CheckFallRespawn()
    {
        if (transform.position.y < -11)
        {
            Teleport(Spawner.Instance.GetRespawnPosition());
        }
    }
    public void Teleport(Vector3 pos)
    {
        Controller.enabled = false;
        transform.position = pos;
        Controller.enabled = true;
    }
    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
    public void CheckGroundedState()
    {
        // set sphere position, with offset
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
            transform.position.z);
        bool isGrounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
            QueryTriggerInteraction.Ignore);
        if (!isGrounded)
        {
            if (checkCoyoteOnce)
            {
                coyoteTime = 0;
                checkCoyoteOnce = false;
            }
            coyoteTime += Time.fixedDeltaTime;
            if (coyoteTime > coyoteTimeMax)
            {
                grounded = false;
            }
        }
        else
        {
            grounded = true;
            checkCoyoteOnce = true;
        }

        _animator.SetBool(_animIDGrounded, grounded);
    }
    public Transform GetItemExaminationPoint()
    {
        return itemExaminationPoint;
    }
    void Die()
    {
        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { "alive", false } });
    }
    public AudioSource GetAudioSource()
    {
        return GetComponent<AudioSource>();
    }
    public void OnFootstep(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            if (FootstepAudioClips.Length > 0)
            {
                var index = UnityEngine.Random.Range(0, FootstepAudioClips.Length);
                AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(Controller.center), FootstepAudioVolume);
            }
        }
    }
    public void OnLand()
    {
        AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(transform.position), FootstepAudioVolume);
        _animator.SetBool("Jump", false);
        _verticalVelocity = 0f;
    }

}