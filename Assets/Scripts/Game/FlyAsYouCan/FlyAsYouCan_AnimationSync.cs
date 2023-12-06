using Photon.Pun;
using UnityEngine;

public class FlyAsYouCan_AnimationSync : MonoBehaviour, IPunObservable
{
    private StickAnimationManager stickAnimationManager;
    private BallAnimationManager ballAnimationManager;
    private void Awake()
    {
        stickAnimationManager = GetComponentInChildren<StickAnimationManager>();
        ballAnimationManager = GetComponentInChildren<BallAnimationManager>();
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //Stick animation sync
            stream.SendNext(stickAnimationManager.animationOrder);
            stream.SendNext(stickAnimationManager.playFrameTimer);

            //Ball animation sync
            stream.SendNext(ballAnimationManager.currentMultiplier);
            stream.SendNext(ballAnimationManager.currentState);
        }
        else
        {
            //Stick animation sync
            int order = (int)stream.ReceiveNext();
            float frameTimer = (float)stream.ReceiveNext();
            if (order != stickAnimationManager.animationOrder || stickAnimationManager.playFrameTimer != frameTimer)
            {
                stickAnimationManager.animationOrder = order;
                stickAnimationManager.playFrameTimer = frameTimer;
                switch (stickAnimationManager.animationOrder)
                {
                    case 0:
                        stickAnimationManager.animator.Play(StickAnimationManager.BEND_STICK, 0, stickAnimationManager.playFrameTimer);
                        break;
                    case 1:
                        stickAnimationManager.animator.Play(StickAnimationManager.RELEASE_STICK, 0, stickAnimationManager.playFrameTimer);
                        break;
                }
            }

            //Ball animation sync
            float multiplier = (float)stream.ReceiveNext();
            var state = (BallAnimationManager.AnimationState)stream.ReceiveNext();
            if (ballAnimationManager.currentMultiplier != multiplier || ballAnimationManager.currentState != state)
            {
                ballAnimationManager.currentState = state;
                ballAnimationManager.currentMultiplier = multiplier;
                ballAnimationManager.Play(state);
                ballAnimationManager.SetBallRotateSpeedMultiplier(multiplier);
            }
        }
    }

}
