﻿using UnityEngine;
using UnityEngine.EventSystems;

public class StickController : MonoBehaviour
{
    [SerializeField] private StickAnimationManager stickAnimationManager;
    [SerializeField] private float touchSensitivity = 10f;
    [SerializeField] private BallController ballController;
    private Vector2 touchStartedPos;
    private Vector2 touchEndedPos;
    private Vector2 lastTouchPos;
    private float stickPosValue = 0f;
    private bool isBallThrowed = false;
    private BirdOwnerAssigner ownerAssigner;
    private void Start()
    {
        ownerAssigner = GetComponentInParent<BirdOwnerAssigner>();
        FlyAsYouCan_GameManager.Instance.OnStateChanged += GameManager_OnStateChanged; // Subscribe to the game state change event
    }

    private void GameManager_OnStateChanged(object sender, FlyAsYouCan_GameManager.State e)
    {
        // Handle game state changes
        switch (e)
        {
            case FlyAsYouCan_GameManager.State.Ready:
                isBallThrowed = false; // Reset the ball thrown flag when the game state changes to ready
                break;

            case FlyAsYouCan_GameManager.State.Playing:
                isBallThrowed = true; // Set the ball thrown flag when the game state changes to playing
                break;
        }
    }

    private void Update()
    {
        if (isBallThrowed || !ownerAssigner.isMine) { return; } // Do nothing if the ball is thrown

//#if UNITY_EDITOR
        // Handle touch input for non-editor platforms
        if (EventSystem.current.currentSelectedGameObject != null) { return; }

        if (Input.GetMouseButtonDown(0))
        {
            touchStartedPos = Input.mousePosition;
            lastTouchPos = touchStartedPos;
        }
        else if (Input.GetMouseButton(0))
        {
            touchEndedPos = Input.mousePosition;
            stickPosValue += CalculateMoveDirection(lastTouchPos, touchEndedPos);
            stickPosValue = ClampValue(stickPosValue, 0f, 1f);
            stickAnimationManager.PlayAt(stickPosValue);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            stickAnimationManager.ReleaseStick();
            ballController.ThrowBall(stickPosValue);
            stickPosValue = 0;
        }
//#else
//        // Handle touch input for the editor
//        if (Input.touchCount > 0)
//        {
//            if (EventSystem.current.currentSelectedGameObject != null) { return; }

//            Touch touch = Input.GetTouch(0);
//            if (touch.phase == TouchPhase.Began)
//            {
//                touchStartedPos = touch.position;
//                lastTouchPos = touchStartedPos;
//            }
//            else if (touch.phase == TouchPhase.Moved)
//            {
//                touchEndedPos = touch.position;
//                stickPosValue += CalculateMoveDirection(lastTouchPos, touchEndedPos);
//                stickPosValue = ClampValue(stickPosValue, 0f, 1f);
//                stickAnimationManager.PlayAt(stickPosValue);
//                lastTouchPos = touchEndedPos;
//            }
//            else if (touch.phase == TouchPhase.Ended)
//            {
//                stickAnimationManager.ReleaseStick();
//                ballController.ThrowBall(stickPosValue);
//                stickPosValue = 0;
//            }
//        }
//#endif
    }

    private float CalculateMoveDirection(Vector2 start, Vector2 end)
    {
        float distance = Mathf.Abs(end.x - start.x);
        if (distance < touchSensitivity)
        {
            return 0f; // No movement
        }
        else
        {
            lastTouchPos = touchEndedPos;
            return Mathf.Sign(end.x - start.x) > 0 ? -0.1f : 0.1f; // Move to the right or left
        }
    }

    private float ClampValue(float value, float min, float max)
    {
        return Mathf.Clamp(value, min, max);
    }
}
