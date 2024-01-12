using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipMovement : MonoBehaviour
{
    Rigidbody body;

    float horizontal;
    float vertical;

    private float runSpeed = 10.0f;
    private float turnSpeed = 100.0f;

    void Start()
    {
        body = GetComponent<Rigidbody>();
    }

    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
    }

    private void FixedUpdate()
    {
        if (vertical != 0)
        {
            if (vertical < 0) vertical /= 5;
            body.velocity = runSpeed * vertical * transform.forward;
        }
        if (vertical != 0) {
            float rot = turnSpeed * Time.deltaTime;
            if (horizontal > 0)
            {
                body.rotation = Quaternion.Euler(0, body.rotation.eulerAngles.y + rot, 0);
                //body.rotation  *= Quaternion.AngleAxis(rot, new Vector3(0, 1, 0));
            }
            else if (horizontal < 0)
            {
                body.rotation = Quaternion.Euler(0, body.rotation.eulerAngles.y - rot, 0);
                //body.rotation  *= Quaternion.AngleAxis(rot, new Vector3(0, -1, 0));
            }
        }
    }
}
