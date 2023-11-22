using UnityEngine;

public class Ground : MonoBehaviour
{
    private bool isCheckable = true;
    private void Start()
    {
        FlyAsYouCan_GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;
    }

    private void GameManager_OnStateChanged(object sender, FlyAsYouCan_GameManager.State e)
    {
        switch(e)
        {
            case FlyAsYouCan_GameManager.State.Playing:
                isCheckable = true; break;
            case FlyAsYouCan_GameManager.State.TryAgain:
                isCheckable = false; break;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        BirdOwnerAssigner assigner = collision.transform.GetComponentInParent<BirdOwnerAssigner>();
        if (assigner != null)
        {
            if (!assigner.isMine)
            {
                Debug.Log("benim deil returnn (ground)");
                return;
            }
        }
        if (isCheckable && collision.transform.TryGetComponent(out BallController ballController))
        {
            FlyAsYouCan_GameManager.Instance.ChangeState(FlyAsYouCan_GameManager.State.TryAgain);
            Invoke(nameof(StartAgain), 1.5f);
        }
    }
    private void StartAgain()
    {
        FlyAsYouCan_GameManager.Instance.ChangeState(FlyAsYouCan_GameManager.State.Ready);
    }
}
