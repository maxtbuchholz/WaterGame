using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleBob : MonoBehaviour
{
    [SerializeField] WaterGetHeight wGetHeight;
    void Update()
    {
        transform.position = new Vector3(transform.position.x, wGetHeight.getWaterHeight(transform.position.x, transform.position.z), transform.position.z);
    }
}
