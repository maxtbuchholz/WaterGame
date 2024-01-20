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


    private float acceleration = 0.6f;
    private float decceleration = 0.3f;
    private float maxSpeed = 4.0f;
    private float maxReverseSpeed = 1.5f;
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
        horizontal = PlayerInput.Instance.GetHorizontal();
        //Debug.Log(horizontal);
        vertical = PlayerInput.Instance.GetVertical();
    }

    private void FixedUpdate()
    {
        if (vertical != 0)              //speed up in either forward or backwards
        {
            float vertSigh = Mathf.Sign(vertical);
            vertical = Mathf.Pow(Mathf.Abs(vertical), 0.5f);
            vertical *= vertSigh;
            float aimSpeed = vertical * maxSpeed;
            if (vertical < 0) aimSpeed = vertical * maxReverseSpeed;
            if(aimSpeed > currSpeed)
            {
                currSpeed += (acceleration * Time.fixedDeltaTime);
                if (Mathf.Abs(currSpeed) < Mathf.Abs(aimSpeed))
                    currSpeed += (decceleration * Time.fixedDeltaTime);
                currSpeed = Mathf.Min(maxSpeed, currSpeed);
            }
            else
            {
                currSpeed -= (acceleration * Time.fixedDeltaTime);
                if(Mathf.Abs(currSpeed) > Mathf.Abs(aimSpeed))
                    currSpeed -= (decceleration * Time.fixedDeltaTime);
                currSpeed = Mathf.Max(-maxReverseSpeed, currSpeed);
            }
            body.velocity = currSpeed * transform.forward;

            //currSpeed += (acceleration * Time.fixedDeltaTime * vertical);
            //if (Mathf.Sign(currSpeed) != Mathf.Sign(vertical)) currSpeed += (((-decceleration) * Mathf.Sign(currSpeed)) * Time.fixedDeltaTime);
            //if (currSpeed > 0) currSpeed = Mathf.Min(currSpeed, maxSpeed);
            //else currSpeed = Mathf.Max(currSpeed, -maxReverseSpeed);
            //body.velocity = currSpeed * transform.forward;


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
            //float turnAngle = 0.0f;
            //turnAngle = Mathf.Min(Mathf.Abs(currSpeed) / maxSpeed, 1.0f);
            //turnAngle *= horizontal;
            //turnAngle *= Mathf.Sign(currSpeed);
            //turnAngle *= 20;
            //Debug.Log(turnAngle);
            //currRotAngle = (turnAngle * Time.fixedDeltaTime) + (currRotAngle * (1 - Time.fixedDeltaTime));
            //float speedBasedTurnSpeed =  Mathf.Lerp(0, turnSpeed, Mathf.Abs(currSpeed) / maxSpeed);
            //float rot = speedBasedTurnSpeed * Time.deltaTime;
            float horSign = Mathf.Sign(horizontal);
            horizontal = Mathf.Pow(Mathf.Abs(horizontal), 0.8f);
            horizontal *= horSign;
            //currRotAngle = (horizontal * Time.fixedDeltaTime) + (currRotAngle * (1 - Time.fixedDeltaTime)); //Mathf.Abs(horizontal);//  0.1f;
            currRotAngle = Mathf.Lerp(currRotAngle, horizontal, Time.fixedDeltaTime / 2);
            currRotAngle *= Mathf.Pow(Mathf.Abs(currSpeed) / maxSpeed,0.1f);
            float rot = currRotAngle / 1.5f;                                    //turning radius, higher is larger radii, 1.5 feels pretty good
            try
            {
                if (horizontal !=  0)
                {
                    body.rotation = Quaternion.Euler(0, body.rotation.eulerAngles.y + rot, 0);
                    shipBody.transform.localRotation = Quaternion.Euler(shipRotation.bobDisplacement.x, body.rotation.eulerAngles.y + rot + shipRotation.bobDisplacement.y, currRotAngle + shipRotation.bobDisplacement.z);
                    //body.rotation  *= Quaternion.AngleAxis(rot, new Vector3(0, 1, 0));
                }
                //else if (horizontal < 0)
                //{
                //    body.rotation = Quaternion.Euler(0, body.rotation.eulerAngles.y - currRotAngle, 0);
                //    shipBody.transform.localRotation = Quaternion.Euler(shipRotation.bobDisplacement.x, (body.rotation.eulerAngles.y - currRotAngle) + shipRotation.bobDisplacement.y, currRotAngle + shipRotation.bobDisplacement.z);
                //    //body.rotation  *= Quaternion.AngleAxis(rot, new Vector3(0, -1, 0));
                //}
                else
                {
                    //currRotAngle = (0 * Time.fixedDeltaTime) + (rot * (1 - Time.fixedDeltaTime));
                    currRotAngle = Mathf.Lerp(currRotAngle, 0, Time.fixedDeltaTime / 2);
                    body.rotation = Quaternion.Euler(0, body.rotation.eulerAngles.y, 0);
                    shipBody.transform.localRotation = Quaternion.Euler(shipRotation.bobDisplacement.x, (body.rotation.eulerAngles.y) + shipRotation.bobDisplacement.y, rot + shipRotation.bobDisplacement.z);
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
