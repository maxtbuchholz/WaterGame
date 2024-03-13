using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretPointer : MonoBehaviour
{
    [SerializeField] public List<TurretController> turrets;
    [SerializeField] public List<DetectHit> detectHit;
}
