using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class RewardCreator : MonoBehaviour
{
    [SerializeField] private Transform rewardPrefab;
    [SerializeField] private ParticleSystem bubbleSplashVFX;
    [SerializeField] private Transform rewardsHolder;
    private int randomNumber;
    private Vector3 rewardPos;
    private List<Transform> rewards = new();
    private Game game;
    private PhotonView PV;
    private void Awake()
    {
        PV = GetComponent<PhotonView>();
        game = GetComponentInParent<Game>();
        PoolHandler.Instance.Create(bubbleSplashVFX.transform, PoolType.BubbleSplash);
        PoolHandler.Instance.Create(rewardPrefab, PoolType.FlyAsYouCanRewardPrefab, default, 350);
    }
    public void Produce()
    {
        int childCount = transform.GetChild(0).childCount;
        float[] xValues = new float[childCount];
        float[] yValues = new float[childCount];
        float[] zValues = new float[childCount];

        for (int i = 0; i < transform.GetChild(0).childCount; i++)
        {
            randomNumber = Random.Range(0, 3);

            if (randomNumber > 1)
                continue;
            Vector3 temp = transform.GetChild(0).GetChild(i).position;
            temp.y += Random.Range(8f, 12f);
            rewardPos = temp;
            xValues[i] = temp.x;
            yValues[i] = temp.y;
            zValues[i] = temp.z;
        }
        if (game.isOnline)
        {
            foreach (var memberId in game.players)
            {
                if (PhotonHandler.Instance.TryGetPlayerByActorNumber(memberId, out var player))
                {
                    PV.RPC(nameof(CreateRewards), player, xValues, yValues, zValues);
                }
            }
        }
        else
        {
            CreateRewards(xValues, yValues, zValues);
        }
    }
    [PunRPC]
    private void CreateRewards(float[] xVals, float[] yVals, float[] zVals)
    {
        if (rewards.Count > 0)
        {
            foreach (var reward in rewards)
            {
                if (reward.gameObject.activeSelf)
                    PoolHandler.Instance.Release(reward, PoolType.FlyAsYouCanRewardPrefab);
            }
        }
        rewards.Clear();
        for (int i = 0; i < xVals.Length; i++)
        {
            if (xVals[i] != 0 && yVals[i] != 0)
            {
                Transform reward = PoolHandler.Instance.Get(PoolType.FlyAsYouCanRewardPrefab);
                reward.SetParent(rewardsHolder);
                rewards.Add(reward);
                reward.position = new Vector3(xVals[i], yVals[i], zVals[i]);
            }
        }
    }
    public void DisableRewardVisual(Transform rewardTransform)
    {
        int rewardOrder = rewards.IndexOf(rewardTransform.parent);
        if (game.isOnline)
        {
            foreach (var memberId in game.players)
            {
                if (PhotonHandler.Instance.TryGetPlayerByActorNumber(memberId, out var player))
                {
                    PV.RPC(nameof(DisableRewardVisualRPC), player, rewardOrder);
                }
            }
        }
        else
            DisableRewardVisualRPC(rewardOrder);
    }
    [PunRPC]
    private void DisableRewardVisualRPC(int rewardOrder)
    {
        if (rewardOrder != -1)
        {
            PoolHandler.Instance.Release(rewards[rewardOrder], PoolType.FlyAsYouCanRewardPrefab);
            Transform splash = PoolHandler.Instance.Get(PoolType.BubbleSplash);
            splash.position = rewards[rewardOrder].position;
            splash.GetComponent<ParticleSystem>().Play();
            PoolHandler.Instance.Release(splash, PoolType.BubbleSplash, .3f);
        }
    }
}
