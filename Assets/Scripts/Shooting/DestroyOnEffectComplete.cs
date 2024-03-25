using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class DestroyOnEffectComplete : MonoBehaviour
{
    List<ParticleSystem> particleSystems = new();
    List<VisualEffect> effectSystems = new();
    float time = 0.0f;
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
        if (ob.TryGetComponent<VisualEffect>(out VisualEffect es))
        {
            effectSystems.Add(es);
        }
        for (int i = 0; i < ob.transform.childCount; i++)
            addParticleSystemWithChildren(ob.transform.GetChild(i).gameObject);
    }
    void Update()
    {
        time += Time.deltaTime;
        if (time < 0.2f) return;
        bool destroy = true;
        foreach(ParticleSystem ps in particleSystems)
        {
            if (ps.isPlaying)
                destroy = false;
        }
        foreach (VisualEffect ps in effectSystems)
        {
            if (ps.aliveParticleCount != 0)
                destroy = false;
        }
        if (time > 5) destroy = true;
        if (destroy)
            Destroy(gameObject);
    }
}
