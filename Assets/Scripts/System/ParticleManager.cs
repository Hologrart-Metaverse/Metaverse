using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.ParticleSystem;
[Serializable]
public class ParticleArgs
{
    public ParticleType particleType;
    public ParticleSystem particle;
}
public enum ParticleType
{
    Explosion,
}
public class ParticleManager : MonoBehaviour
{
    public static ParticleManager Instance;
    [SerializeField] private List<ParticleArgs> particleArgs = new();
    private void Awake()
    {
        Instance = this;
    }
    public void Play(ParticleType particleType, Vector3 pos, Quaternion rot = default, float releaseAfterDelay = 0f)
    {
        if (rot == default)
            rot = Quaternion.identity;

        ParticleSystem particleSystem = GetParticle(particleType);
        particleSystem.transform.position = pos;
        particleSystem.transform.rotation = rot;
        particleSystem.Play();
        if (releaseAfterDelay > 0f)
        {
            PoolHandler.Instance.Release(particleSystem.transform, GetPoolType(particleType), releaseAfterDelay);
        }
    }
    public PoolType GetPoolType(ParticleType particleType)
    {
        PoolType poolType = default;
        switch (particleType)
        {
            case ParticleType.Explosion:
                poolType = PoolType.ExplosionEffect; break;
        }
        return poolType;
    }
    private ParticleSystem GetParticle(ParticleType particleType)
    {
        Transform particle;
        PoolType poolType = GetPoolType(particleType);
        particle = PoolHandler.Instance.Get(poolType);
        if (particle == null)
        {
            PoolHandler.Instance.Create(particleArgs.Where(x => x.particleType == particleType).FirstOrDefault().particle.transform, poolType);
            particle = PoolHandler.Instance.Get(poolType);
        }
        return particle.GetComponent<ParticleSystem>();
    }
}
