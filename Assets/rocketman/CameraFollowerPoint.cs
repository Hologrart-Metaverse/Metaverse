using UnityEngine;

public class CameraFollowerPoint : MonoBehaviour
{
    [SerializeField] private Transform ballTransform; 
    private Vector3 lockedPosition;
    private Quaternion lockedRotation; 
    private bool isGameOver;
    BirdOwnerAssigner ownerAssigner;
    [SerializeField] private float followSmoothness = 4f;
    [SerializeField] private float rotSmoothness = 4f;
    private void Awake()
    {
        ownerAssigner = GetComponentInParent<BirdOwnerAssigner>();
    }
    private void Update()
    {
        if (!ownerAssigner.isMine)
            return;
        // Update the camera's position based on the ball's position, unless the game is over
        Vector3 pos = isGameOver ? lockedPosition : ballTransform.position;
        transform.position = Vector3.Lerp(transform.position, pos, Time.fixedDeltaTime * followSmoothness);
    }

    private void LateUpdate()
    {
        if (!ownerAssigner.isMine)
            return;
        // Calculate the camera's rotation based on the ball's rotation, but only if the game is not over
        Quaternion rotation = !isGameOver ? Quaternion.Euler(transform.rotation.eulerAngles.x, ballTransform.rotation.eulerAngles.y, 0f) : lockedRotation;

        // Smoothly interpolate the camera's rotation towards the calculated rotation
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.fixedDeltaTime * rotSmoothness);
    }

    private void Start()
    {
        FlyAsYouCan_GameManager.Instance.OnStateChanged += GameManager_OnStateChanged; // Subscribe to the game state change event
    }

    private void GameManager_OnStateChanged(object sender, FlyAsYouCan_GameManager.State e)
    {
        // Handle game state changes
        switch (e)
        {
            case FlyAsYouCan_GameManager.State.Ready:
                isGameOver = false; // Reset the game over flag when the game state changes to ready
                break;

            case FlyAsYouCan_GameManager.State.TryAgain:
                // Lock the camera's position and rotation when the game state changes to game over
                lockedPosition = transform.position;
                lockedRotation = transform.rotation;
                isGameOver = true; // Set the game over flag
                break;
        }
    }
}
