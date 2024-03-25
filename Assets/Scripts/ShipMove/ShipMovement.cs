using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class ShipMovement : MonoBehaviour
{
    Rigidbody body;
    [SerializeField] public ShipRotation shipRotation;
    [SerializeField] public GameObject shipBody;
    [SerializeField] public List<ParticleSystem> backParticles;
    [SerializeField] public ParticleSystem frontParticle;
    [SerializeField] public List<VisualEffect> funnelParticles;
    private float amountFunnelParticles;
    private float amountBackParticles;
    private float amountFrontParticles;
    public int teamId = -1;
    [HideInInspector] public float currSpeedPercentage = 0;
    private SaveData saveData;

    float horizontal;
    float vertical;


    private float acceleration = 1.5f;
    private float decceleration = 1.5f;
    private float maxSpeed = 2.0f;                  //set by level
    private float maxReverseSpeed = 1.0f;
    private float currAccel = 0.0f;
    private float currRotAngle = 0.0f;
    private Vector3 currVelocity = Vector3.zero;
    public bool isPlayer = false;

    public void Start()
    {
        if (frontParticle == null) return;
        saveData = SaveData.Instance;
        body = GetComponent<Rigidbody>();
        amountFunnelParticles = funnelParticles[0].GetFloat("SpawnRate");
        amountBackParticles = backParticles[0].emissionRate;
        amountFrontParticles = frontParticle.emissionRate;
        if (isPlayer)
        {
            transform.position = saveData.GetPlayerPos();
            body.rotation = Quaternion.Euler(0, saveData.GetPlayerRot(), 0);
        }
        //Debug.Log(amountFrontParticles);
    }
    public void SetMaxSpeed(float speed)
    {
        maxSpeed = speed;
        maxReverseSpeed = speed / 2;
    }
    public void SetControlsNN(float horizontal, float vertical)
    {
        this.horizontal = horizontal;
        this.vertical = vertical;
    }
    void Update()
    {
        if (isPlayer)
        {
            horizontal = PlayerInput.Instance.GetHorizontal();
            //Debug.Log(horizontal);
            vertical = PlayerInput.Instance.GetVertical();
            saveData.SetPlayerPos(transform.position);
            saveData.SetPlayerRot(body.rotation.eulerAngles.y);
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        Vector3 imp = transform.position - collision.contacts[0].point;// collision.impulse;
        //Debug.Log(imp);
        float force = Mathf.Min(collision.impulse.magnitude / 4, 5f);
        imp.x *= force + 1;
        imp.y = 0;
        imp.z *= force + 1;
        body.velocity += imp;
        //body.AddExplosionForce(2, collision.contacts[0].point, 10, 0, ForceMode.VelocityChange);
        //currSpeed = 0;
    }
    float prevLean = 0;
    float prevHor = 0;
    private void FixedUpdate()
    {
        horizontal = Mathf.Lerp(prevHor, horizontal, Time.deltaTime / 0.5f);
        prevHor = horizontal;
        Vector3 setSpeed = transform.InverseTransformVector(body.velocity);
        float currRBSpeed = setSpeed.z;
        setSpeed *= (1 - (Time.fixedDeltaTime / 0.02f));
        setSpeed.z = currRBSpeed;
        body.velocity = transform.TransformVector(setSpeed);
        currSpeedPercentage = currRBSpeed / maxSpeed;
        if (vertical != 0)              //speed up in either forward or backwards
        {
            float vertSigh = Mathf.Sign(vertical);
            vertical = Mathf.Pow(Mathf.Abs(vertical), 0.5f);
            vertical *= vertSigh;
            float aimSpeed = vertical * maxSpeed;
            if (vertical < 0) aimSpeed = vertical * maxReverseSpeed;
            if(aimSpeed > currRBSpeed)
            {
                currAccel = (acceleration * Time.fixedDeltaTime);
                //if ((Mathf.Abs(currRBSpeed) < Mathf.Abs(aimSpeed)) && (Mathf.Sign(currRBSpeed) != Mathf.Sign(aimSpeed)))
                //    currAccel += (decceleration * Time.fixedDeltaTime);
                if (currRBSpeed + currAccel > aimSpeed) currAccel = aimSpeed - currRBSpeed;
            }
            else
            {
                currAccel = -(acceleration * Time.fixedDeltaTime);
                //if ((Mathf.Abs(currRBSpeed) < Mathf.Abs(aimSpeed)) && (Mathf.Sign(currRBSpeed) != Mathf.Sign(aimSpeed)))
                //    currAccel -= (decceleration * Time.fixedDeltaTime);
                if (currRBSpeed + currAccel < aimSpeed) currAccel = currRBSpeed - aimSpeed;
            }
            body.velocity += (currAccel * transform.forward);

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
        else if(currRBSpeed != 0)
        {
            if (currRBSpeed < 0)
            {
                //currSpeed += (decceleration * Time.fixedDeltaTime);
                //if (currSpeed > 0) currSpeed = 0;
                //Vector3 vel = new Vector3(0, 0, currSpeed); 
                //currVelocity = Vector3.Lerp(vel, currVelocity, Time.fixedDeltaTime);
                //body.velocity = currVelocity;

                currAccel = (decceleration * Time.fixedDeltaTime);
                if (currRBSpeed + currAccel > 0) currAccel = 0 - currRBSpeed;
            }
            else
            {
                //currSpeed -= (decceleration * Time.fixedDeltaTime);
                //if (currSpeed < 0) currSpeed = 0;
                //Vector3 vel = new Vector3(0, 0, currSpeed);
                //currVelocity = Vector3.Lerp(vel, currVelocity, Time.fixedDeltaTime);
                //body.velocity = currVelocity;

                currAccel = -(decceleration * Time.fixedDeltaTime);
                if (currRBSpeed + currAccel < 0) currAccel = currRBSpeed;
            }
            body.velocity += (currAccel * transform.forward);
        }
        else
        {
            Vector3 vel = new Vector3(0, 0, 0);
            currVelocity = Vector3.Lerp(vel, currVelocity, Time.fixedDeltaTime);
            body.velocity = currVelocity;
        }
        if ((vertical != 0) || true)
        {
            //float turnAngle = 0.0f;
            //turnAngle = Mathf.Min(Mathf.Abs(currSpeed) / maxSpeed, 1.0f);
            //turnAngle *= horizontal;
            //turnAngle *= Mathf.Sign(currSpeed);
            //turnAngle *= 20;
            //Debug.Log(turnAngle);
            //currRotAngle = (turnAngle * Time.fixedDeltaTime) + (currRotAngle * (1 - Time.fixedDeltaTime));
            //float speedBasedTurnSpeed =  Mathf.Lerp(0, turnSpeed, Mathf.Abs(currSpeed) / maxSpeed);
            //float rot = speedBasedTurnSpeed * Time.deltaTime;
            //float horSign = Mathf.Sign(horizontal);
            //horizontal = Mathf.Pow(Mathf.Abs(horizontal), 0.8f);
            //horizontal *= horSign;
            //currRotAngle = (horizontal * Time.fixedDeltaTime) + (currRotAngle * (1 - Time.fixedDeltaTime)); //Mathf.Abs(horizontal);//  0.1f;
            horizontal *= currRBSpeed / maxSpeed;
            float t = Time.fixedDeltaTime;
            //Debug.Log(currRotAngle);
            currRotAngle = Mathf.Lerp(currRotAngle, horizontal, 1.0f);// horizontal;// (t * horizontal) + ((1 - t) * currRotAngle);
            //currRotAngle = Mathf.Lerp(currRotAngle, horizontal, Time.fixedDeltaTime);// Mathf.Lerp(currRotAngle, horizontal, Time.fixedDeltaTime / 2);
            //currRotAngle *= Mathf.Pow(Mathf.Abs(currRBSpeed) / maxSpeed,0.1f);
            float rot = currRotAngle /= 3f;                                    //turning radius, higher is larger radii, 1.5 feels pretty good
            prevLean = Mathf.Lerp(prevLean, rot * 15, Time.fixedDeltaTime * 2);
            try
            {
                if (horizontal != 0)
                {
                    body.rotation = Quaternion.Euler(0, body.rotation.eulerAngles.y + rot, 0);
                    shipBody.transform.localRotation = Quaternion.Euler(shipRotation.bobDisplacement.x, body.rotation.eulerAngles.y + rot + shipRotation.bobDisplacement.y, prevLean + currRotAngle + shipRotation.bobDisplacement.z);
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
                    //currRotAngle = Mathf.Lerp(currRotAngle, 0, Time.fixedDeltaTime / 2);
                    body.rotation = Quaternion.Euler(0, body.rotation.eulerAngles.y, 0);
                    shipBody.transform.localRotation = Quaternion.Euler(shipRotation.bobDisplacement.x, (body.rotation.eulerAngles.y) + shipRotation.bobDisplacement.y, prevLean + shipRotation.bobDisplacement.z);
                }
            }
            catch (System.Exception e)
            {
                Debug.Log(e.Message);
            }
        }
        //else
        //{
        //    float turnAngle = 0.0f;
        //    currRotAngle = (turnAngle * Time.fixedDeltaTime) + (currRotAngle * (1 - Time.fixedDeltaTime));
        //    body.rotation = Quaternion.Euler(0, body.rotation.eulerAngles.y, 0);
        //    shipBody.transform.localRotation = Quaternion.Euler(shipRotation.bobDisplacement.x, (body.rotation.eulerAngles.y) + shipRotation.bobDisplacement.y, currRotAngle + shipRotation.bobDisplacement.z);
        //}
        ///////////set ship particles
        frontParticle.emissionRate = Mathf.Lerp(0, amountFrontParticles, currRBSpeed / maxSpeed);
        float smokeEmmission = Mathf.Abs(vertical) / 1;
        //smokeEmmission = Mathf.Pow(smokeEmmission, 0.5f);
        smokeEmmission *= amountFunnelParticles;
        foreach (VisualEffect vFX in funnelParticles)
            vFX.SetFloat("SpawnRate", smokeEmmission);
        float backParRate = Mathf.Lerp(0, amountBackParticles, vertical / 1);
        foreach (ParticleSystem ps in backParticles)
            ps.emissionRate = backParRate;
    }
}
