using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackBrain : MonoBehaviour
{
    [SerializeField] ShipMovement shipMovement;
    private void Update()
    {
        if (!ena) return;
        float vertical = -1;
        float horizontal = -0.5f;
        shipMovement.SetControlsNN(horizontal, vertical);
    }
    public bool ena = true;
    public void SetEnables(bool enabled)
    {
        ena = enabled;
    }
}
