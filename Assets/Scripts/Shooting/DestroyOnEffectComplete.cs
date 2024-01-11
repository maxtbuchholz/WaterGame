using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnEffectComplete : MonoBehaviour
{
    List<ParticleSystem> particleSystems = new();
    void Start()
    {
        addParticleSystemWithChildren(gameObject);
    }
    void addParticleSystemWithChildren(GameObject ob)
    {
        if (ob.TryGetComponent<ParticleSystem>(out ParticleSystem ps))
        {
            particleSystems.Add(ps);
        }
        for (int i = 0; i < ob.transform.childCount; i++)
            addParticleSystemWithChildren(ob.transform.GetChild(i).gameObject);
    }
    void Update()
    {
        bool destroy = true;
        foreach(ParticleSystem ps in particleSystems)
        {
            if (ps.isPlaying)
                destroy = false;
        }
        if (destroy)
            Destroy(gameObject);
    }
}
