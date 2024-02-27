using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    [SerializeField] float rotationSpeed = -400;

    float rotation = 0;
    void Update()
    {
        transform.rotation = Quaternion.Euler(0, 0, rotation);
        rotation += (Time.deltaTime * rotationSpeed);
        rotation %= 360;
    }
}
