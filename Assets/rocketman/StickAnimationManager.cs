using Photon.Pun;
using UnityEngine;

public class StickAnimationManager : MonoBehaviour
{
    internal const string BEND_STICK = "Armature|Bend_Stick";
    internal const string RELEASE_STICK = "Armature|Release_Stick"; 
    internal Animator animator; 
    internal float playFrameTimer;
    internal int animationOrder;
    private void Awake()
    {
        animator = GetComponent<Animator>(); // Get the Animator component on Awake
    }

    public void PlayAt(float _time)
    {
        playFrameTimer = _time;
        // Play the animation from the desired frame
        animationOrder = 0;
        animator.Play(BEND_STICK, 0, playFrameTimer);
    }

    public void ReleaseStick()
    {
        playFrameTimer = CalculateReleasePosition(); // Calculate the release position
        animationOrder = 1;
        animator.Play(RELEASE_STICK, 0, playFrameTimer); // Play the release stick animation at the calculated position
    }

    private float CalculateReleasePosition()
    {
        // Calculate the position in the release animation based on stick bending position
        // The stick's upright position corresponds to 23% of the animation, so we calculate accordingly
        return (1 - playFrameTimer) * 23 / 100;
    }

    //public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    //{
    //    if (stream.IsWriting)
    //    {
    //        Debug.Log("yazaýyom yine");
    //        stream.SendNext(animationOrder);
    //        stream.SendNext(playFrameTimer);
    //    }
    //    else
    //    {
    //        int order = (int)stream.ReceiveNext();
    //        float frameTimer = (float)stream.ReceiveNext();
    //        if (order != animationOrder || playFrameTimer != frameTimer)
    //        {
    //            animationOrder = order;
    //            playFrameTimer = frameTimer;
    //            switch (animationOrder)
    //            {
    //                case 0:
    //                    animator.Play(BEND_STICK, 0, playFrameTimer);
    //                    break;
    //                case 1:
    //                    animator.Play(RELEASE_STICK, 0, playFrameTimer);
    //                    break;
    //            }
    //        }
    //    }
    //}
}
