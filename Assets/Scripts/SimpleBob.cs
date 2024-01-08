using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleBob : MonoBehaviour
{
    [SerializeField] WaterGetHeight wGetHeight;
    void FixedUpdate()
    {
        GetComponent<Rigidbody>().MovePosition(new Vector3(transform.position.x, (wGetHeight.getWaterHeight(transform.position.x, transform.position.z) + (3 * transform.position.y)) / 4, transform.position.z));
        //transform.position = new Vector3(transform.position.x, wGetHeight.getWaterHeight(transform.position.x, transform.position.z), transform.position.z);
    }
}
