using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoamCameraFollow : MonoBehaviour
{
    [SerializeField] Transform followOb;

    void Update()
    {
        transform.position = new Vector3(followOb.position.x, transform.position.y, followOb.position.z);
    }
}
