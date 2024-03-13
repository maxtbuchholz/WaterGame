using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectsPointer : MonoBehaviour
{
    [SerializeField] public List<ParticleSystem> backParticles;
    [SerializeField] public ParticleSystem frontParticles;
    [SerializeField] public FrontTrail frontTrail;
}
