using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipMovement : MonoBehaviour
{
    Rigidbody body;
    [SerializeField] ShipRotation shipRotation;
    [SerializeField] GameObject shipBody;
    [SerializeField] List<ParticleSystem> backParticles;
    [SerializeField] ParticleSystem frontParticle;
    private float amountBackParticles;
    private float amountFrontParticles;

    float horizontal;
    float vertical;

    private float turnSpeed = 40.0f;

    private float acceleration = 1.0f;
    private float decceleration = 2.0f;
    private float maxSpeed = 5.0f;
    private float maxReverseSpeed = 2.5f;
    private float currSpeed = 0.0f;
    private float currRotAngle = 0.0f;

    void Start()
    {
        body = GetComponent<Rigidbody>();
        amountBackParticles = backParticles[0].emissionRate;
        amountFrontParticles = frontParticle.emissionRate;
        //Debug.Log(amountFrontParticles);
    }

    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        //Debug.Log(horizontal);
        vertical = Input.GetAxisRaw("Vertical");
    }

    private void FixedUpdate()
    {
        if (vertical != 0)              //speed up in either forward or backwards
        {
            currSpeed += (acceleration * Time.fixedDeltaTime * vertical);
            if (currSpeed > 0) currSpeed = Mathf.Min(currSpeed, maxSpeed);
            else currSpeed = Mathf.Max(currSpeed, -maxReverseSpeed);
            body.velocity = currSpeed * transform.forward;
            //Debug.Log(currSpeed);
            //if (vertical < 0) vertical /= 5;
            //if (Mathf.Sign(vertical) != Mathf.Sign(currSpeed)) currSpeed = 0;
            //currSpeed += acceleration * Time.fixedDeltaTime * vertical;
            //currSpeed = Mathf.Min(maxSpeed, currSpeed);
            //body.velocity = currSpeed * transform.forward;
            ////body.velocity = runSpeed * vertical * transform.forward;
        }
        else if(currSpeed != 0)
        {
            if(currSpeed < 0)
            {
                currSpeed += (decceleration * Time.fixedDeltaTime);
                if (currSpeed > 0) currSpeed = 0;
                body.velocity = currSpeed * transform.forward;
            }
            else
            {
                currSpeed -= (decceleration * Time.fixedDeltaTime);
                if (currSpeed < 0) currSpeed = 0;
                body.velocity = currSpeed * transform.forward;
            }
        }
        if (vertical != 0) {
            float turnAngle = 0.0f;
            turnAngle = Mathf.Min(Mathf.Abs(currSpeed) / maxSpeed, 1.0f);
            turnAngle *= horizontal;
            turnAngle *= Mathf.Sign(currSpeed);
            turnAngle *= 20;
            currRotAngle = (turnAngle * Time.fixedDeltaTime) + (currRotAngle * (1 - Time.fixedDeltaTime));
            float speedBasedTurnSpeed = Mathf.Lerp(0, turnSpeed, Mathf.Abs(currSpeed) / maxSpeed);
            float rot = speedBasedTurnSpeed * Time.deltaTime;
            try
            {
                if (horizontal > 0)
                {
                    body.rotation = Quaternion.Euler(0, body.rotation.eulerAngles.y + rot, 0);
                    shipBody.transform.localRotation = Quaternion.Euler(shipRotation.bobDisplacement.x, body.rotation.eulerAngles.y + rot + shipRotation.bobDisplacement.y, currRotAngle + shipRotation.bobDisplacement.z);
                    //body.rotation  *= Quaternion.AngleAxis(rot, new Vector3(0, 1, 0));
                }
                else if (horizontal < 0)
                {
                    body.rotation = Quaternion.Euler(0, body.rotation.eulerAngles.y - rot, 0);
                    shipBody.transform.localRotation = Quaternion.Euler(shipRotation.bobDisplacement.x, (body.rotation.eulerAngles.y - rot) + shipRotation.bobDisplacement.y, currRotAngle + shipRotation.bobDisplacement.z);
                    //body.rotation  *= Quaternion.AngleAxis(rot, new Vector3(0, -1, 0));
                }
                else
                {
                    currRotAngle = (turnAngle * Time.fixedDeltaTime) + (currRotAngle * (1 - Time.fixedDeltaTime));
                    body.rotation = Quaternion.Euler(0, body.rotation.eulerAngles.y, 0);
                    shipBody.transform.localRotation = Quaternion.Euler(shipRotation.bobDisplacement.x, (body.rotation.eulerAngles.y) + shipRotation.bobDisplacement.y, currRotAngle + shipRotation.bobDisplacement.z);
                }
            }
            catch(System.Exception e)
            {
                Debug.Log(e.Message);
            }
        }
        else
        {
            float turnAngle = 0.0f;
            currRotAngle = (turnAngle * Time.fixedDeltaTime) + (currRotAngle * (1 - Time.fixedDeltaTime));
            body.rotation = Quaternion.Euler(0, body.rotation.eulerAngles.y, 0);
            shipBody.transform.localRotation = Quaternion.Euler(shipRotation.bobDisplacement.x, (body.rotation.eulerAngles.y) + shipRotation.bobDisplacement.y, currRotAngle + shipRotation.bobDisplacement.z);
        }
        ///////////set ship particles
        frontParticle.emissionRate = Mathf.Lerp(0, amountFrontParticles, currSpeed / maxSpeed);
        float backParRate = Mathf.Lerp(0, amountBackParticles, vertical / 1);
        foreach (ParticleSystem ps in backParticles)
            ps.emissionRate = backParRate;
    }
}
