using Cinemachine;
using UnityEngine;

public class PlayerFollowCamera : MonoBehaviour
{
    public static PlayerFollowCamera Instance;
    private CinemachineVirtualCamera cinemachine;
    public void AssignPlayerToFollow(Transform followTransform)
    {
        cinemachine.Follow = followTransform;
    }
}
