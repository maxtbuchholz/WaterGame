using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectHit : MonoBehaviour
{
    [SerializeField] public HealthController healthController;
    public void DealtDamage(float damage, int teamID)
    {
        healthController.EffectHealth(damage, teamID);
    }
}
