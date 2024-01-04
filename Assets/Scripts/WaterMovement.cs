using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterMovement : MonoBehaviour
{
    // Start is called before the first frame update
    private float waveSpeed = 0.75f;
    private float waveHeight = 0.10f;
    private float originalHeight;
    void Start()
    {
        originalHeight = transform.position.y;
        //maxWaveHeight = maxWaveHeightDiff + originalHeight;
    }

    // Update is called once per frame
    void Update()
    {
    }
    float totalSinTime = 0f;
    private void FixedUpdate()
    {
        totalSinTime = ((((float)DateTime.Now.Hour * 60.0f) + (float)DateTime.Now.Minute) * 60.0f) + (float)DateTime.Now.Second + ((float)DateTime.Now.Millisecond / 1000.0f);
        transform.position = new Vector3(transform.position.x, (waveHeight * Mathf.Sin(waveSpeed * totalSinTime)) + originalHeight, transform.position.z);
        //if (goingUp)
        //{
        //    transform.position = new Vector3(transform.position.x, (waveSpeed * Time.deltaTime) + transform.position.y, transform.position.z);
        //    if (transform.position.y > maxWaveHeight) goingUp = false;
        //}
        //else
        //{
        //    transform.position = new Vector3(transform.position.x, (-waveSpeed * Time.deltaTime) + transform.position.y, transform.position.z);
        //    if (transform.position.y < originalHeight) goingUp = true;
        //}
    }
}
