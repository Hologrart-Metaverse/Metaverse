using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorEventHandler : MonoBehaviour
{
    private PlayerController playerController;
    private void Awake()
    {
        playerController = transform.GetComponentInParent<PlayerController>();
    }
    private void OnFootstep(AnimationEvent animationEvent)
    {
        playerController.OnFootstep(animationEvent);
    }
    private void OnLand()
    {
        playerController.OnLand();
    }

}
