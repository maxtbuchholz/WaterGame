using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using System;
using static UnityEngine.GraphicsBuffer;

public class TurretController : MonoBehaviour
{
    [SerializeField] ShipValueControl shipValues;
    [SerializeField] FortValues fortValues;
    [SerializeField] GameObject projectile;
    [SerializeField] GameObject turret;
    [SerializeField] GameObject barrel;
    [SerializeField] GameObject shootPoint;
    Vector3[] trajectory = new Vector3[] { };
    private float force = 100;
    private float maxDistance;
    Dictionary<float, float> cannonDistanceToTime = new();
    private void Start()
    {
        maxDistance = Mathf.Pow(force, 2) / 9.81f;
        bool keepSearching = true;
        float step = 1.0f;
        float currStep = 0.0f;
        float forceSqr = force * force;
        while (keepSearching)
        {
            float angle = Mathf.Asin((9.81f * currStep) / (forceSqr)) / 2;
            float timeToTarget = currStep / (Mathf.Cos(angle) * force);
            cannonDistanceToTime.Add(currStep, timeToTarget);
            currStep += step;
            if (currStep > maxDistance)
                keepSearching = false;
        }
    }
    public bool ShootProjectile(Vector3 targetPos)     //returns okay if okay to shoot and shot
    {
        //float force = 100;
        Vector3 vDistance = (targetPos - shootPoint.transform.position);
        vDistance.y = -vDistance.y;
        Vector3 normalDist = (new Vector3(targetPos.x, 0, targetPos.z) - new Vector3(shootPoint.transform.position.x, 0, shootPoint.transform.position.z)).normalized;
        float d = Mathf.Pow(Mathf.Pow(vDistance.x, 2) + Mathf.Pow(vDistance.z, 2), 0.5f);
        float angle = GetAngle(9.81f, force, d, vDistance.y);
        if (float.IsNaN(angle)) return false;
        float rotY = Mathf.Atan2(normalDist.x, normalDist.z) * Mathf.Rad2Deg;
        normalDist.y = Mathf.Sin(angle);
        turret.transform.rotation = Quaternion.Euler(0, rotY, 0);
        barrel.transform.localRotation = Quaternion.Euler(-angle * Mathf.Rad2Deg, 0, 0);
        Debug.DrawLine(shootPoint.transform.position, (normalDist * 5f) + shootPoint.transform.position, Color.red);
        RaycastHit[] hits = (Physics.RaycastAll(shootPoint.transform.position, normalDist, 5f));
        foreach (RaycastHit hit in hits)
            if (!hit.collider.CompareTag("IslandOuterCollider"))
                return false;
        GameObject proj = GameObject.Instantiate(projectile);
        if(shipValues != null)
            proj.GetComponent<ProjectileHit>().shipParts = shipValues.shipParts;
        else if (fortValues != null)
            proj.GetComponent<ProjectileHit>().shipParts = fortValues.fortParts;
        proj.transform.position = shootPoint.transform.position;
        proj.GetComponent<Rigidbody>().velocity = normalDist * force;
        //Debug.Break();
        return true;
    }
    public bool ShootProjectile(Vector2 targetPos, Vector2 targetVelocity)          //attackin a ship with a velocity
    {
        //float force = 100;
        Vector3 oneSecOut = targetPos + targetVelocity;
        float currDst = Vector2.Distance(targetPos, transform.position);
        float diffInDst = Vector2.Distance(oneSecOut, transform.position) - currDst;
        Debug.Log(diffInDst);



        float closestTime = 0;
        float closestDst = 0;
        float closestDstDiff = -1;
        foreach(KeyValuePair<float, float> dstTime in cannonDistanceToTime)
        {
            float tempTarDst = currDst + (diffInDst * dstTime.Value);
            float tempDiff = Mathf.Abs(tempTarDst - dstTime.Key);
            if((tempDiff < closestDstDiff) || (closestDstDiff == -1))
            {
                closestDstDiff = tempDiff;
                closestTime = dstTime.Value;
                closestDst = dstTime.Key;
            }
        }
        targetPos = targetPos + (targetVelocity * closestTime);

        float angle = (Mathf.Asin((9.18F * closestDst) / (Mathf.Pow(force, 2)))) / 2;
        Vector3 normalDist = (new Vector3(targetPos.x, 0, targetPos.y) - new Vector3(shootPoint.transform.position.x, 0, shootPoint.transform.position.z)).normalized;
        float rotY = Mathf.Atan2(normalDist.x, normalDist.z) * Mathf.Rad2Deg;
        normalDist.y = Mathf.Sin(angle);
        turret.transform.rotation = Quaternion.Euler(0, rotY, 0);
        barrel.transform.localRotation = Quaternion.Euler(-angle * Mathf.Rad2Deg, 0, 0);


        GameObject proj = GameObject.Instantiate(projectile);
        if (shipValues != null)
            proj.GetComponent<ProjectileHit>().shipParts = shipValues.shipParts;
        else if (fortValues != null)
            proj.GetComponent<ProjectileHit>().shipParts = fortValues.fortParts;
        proj.transform.position = shootPoint.transform.position;
        proj.GetComponent<Rigidbody>().velocity = normalDist * force;
        //Debug.Break();
        return true;
    }
    float GetAngle(float g, float V, float d, float h)
    {
        int flip = 1;
        if ((h > 0) && (d < (V * Mathf.Pow(Mathf.Abs(2 * h / g), 0.5f))))
        {
            g = -g;
            h = -h;
            flip = -1;
        }
        float phase = Mathf.Atan2(d, Mathf.Abs(h));
        float denom = Mathf.Pow(h * h + d * d, 0.5f);
        float coef = (g * d * d) / (V * V) - h;
        float angle = (Mathf.PI / 2) - (Mathf.Acos(coef / denom) + phase) / 2;
        float time = d / (V * Mathf.Cos(angle));
        return angle * flip;
    }
    void GetWholeTrajectory(Vector3 hitPos)
    {
        trajectory = HitObjectDiffHeight(1000, 100, hitPos);
    }
    Vector3[] HitObjectDiffHeight(int locCount, float locForce, Vector3 targetPos)
    {
        Vector3 vDistance = (targetPos - transform.position);
        vDistance.y = -vDistance.y;
        Vector3 normalDist = (new Vector3(targetPos.x, 0, targetPos.z) - new Vector3(transform.position.x, 0, transform.position.z)).normalized;
        float d = Mathf.Pow(Mathf.Pow(vDistance.x, 2) + Mathf.Pow(vDistance.z, 2), 0.5f);
        float angle = GetAngle(9.81f, locForce, d, vDistance.y);
        float[] points = GetTrajectoryPoints(locCount, locForce, angle, 0);
        Vector3[] verts = new Vector3[locCount];
        for (int x = 0; x < locCount; x++)
        {
            verts[x] = new Vector3(transform.position.x + (normalDist.x * x), points[x], transform.position.z + (normalDist.z * x));
        }
        return verts;
    }
    float[] GetTrajectoryPoints(int count, float force, float angle, float initialHeight)   //meters I guess, angle is in radians
    {
        float[] points = new float[count];
        for (int x = 0; x < points.Length; x++)
        {
            float y = initialHeight + (x * Mathf.Tan(angle));
            y -= (9.81f * (Mathf.Pow(x, 2) / (2 * Mathf.Pow(force, 2) * Mathf.Pow(Mathf.Cos(angle), 2))));
            points[x] = y;
        }

        return points;
    }
    float GetHeightAtTime(float force, float angle, float initialHeight, float time)
    {
        float y = initialHeight + (time * Mathf.Tan(angle));
        y -= (9.81f * (Mathf.Pow(time, 2) / (2 * Mathf.Pow(force, 2) * Mathf.Pow(Mathf.Cos(angle), 2))));
        return y;
    }
}
