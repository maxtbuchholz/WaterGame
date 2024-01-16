using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class TurretController : MonoBehaviour
{
    [SerializeField] Camera camera;
    [SerializeField] ShipValueControl shipValues;
    [SerializeField] GameObject projectile;
    [SerializeField] GameObject turret;
    [SerializeField] GameObject barrel;
    [SerializeField] GameObject shootPoint;
    Vector3[] trajectory = new Vector3[] { };
    public bool ShootProjectile(Vector3 targetPos)     //returns okay if okay to shoot and shot
    {
        float force = 50;
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
        //Debug.Break();
        if (Physics.Raycast(shootPoint.transform.position, normalDist, 5f))
            return false;
        
        GameObject proj = GameObject.Instantiate(projectile);
        proj.GetComponent<ProjectileHit>().shipParts = shipValues.shipParts;
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