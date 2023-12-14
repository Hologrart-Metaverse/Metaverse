using UnityEngine;

public class Ground : MonoBehaviour
{
    private bool isCheckable = true;
    [SerializeField] private Transform splashEffect;
    private void Start()
    {
        FlyAsYouCan_GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;
        PoolHandler.Instance.Create(splashEffect, PoolType.WaterSplash);
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
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
        Transform splashTransform = PoolHandler.Instance.Get(PoolType.WaterSplash);
        splashTransform.transform.position = other.transform.position;
        splashTransform.GetChild(0).GetComponent<ParticleSystem>().Play();
        PoolHandler.Instance.Release(splashTransform, PoolType.WaterSplash, 1f);
    }
    private void OnCollisionEnter(Collision collision)
    {
        BirdOwnerAssigner assigner = collision.transform.GetComponentInParent<BirdOwnerAssigner>();
        if (assigner != null)
        {
            if (!assigner.isMine)
            {
                return;
            }
        }
        if (isCheckable && collision.transform.TryGetComponent(out BallController ballController))
        {
            FlyAsYouCan_GameManager.Instance.ChangeState(FlyAsYouCan_GameManager.State.TryAgain);
            Invoke(nameof(StartAgain), .75f);
        }
    }
    private void StartAgain()
    {
        FlyAsYouCan_GameManager.Instance.ChangeState(FlyAsYouCan_GameManager.State.Ready);
    }
}
