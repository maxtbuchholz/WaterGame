using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using System;
using static UnityEngine.GraphicsBuffer;

public class TurretController : MonoBehaviour
{
    [SerializeField] public ShipValueControl shipValues;
    [SerializeField] public FortValues fortValues;
    [SerializeField] GameObject projectile;
    [SerializeField] GameObject turret;
    [SerializeField] GameObject barrel;
    [SerializeField] GameObject shootPoint;
    [SerializeField] GameObject turretCube;
    [SerializeField] GameObject barrelRotationPoint;
    [SerializeField] float distanceMultiplyer = 1;
    [SerializeField] float distanceAdd = 0;
    [SerializeField] float radAngleAdd = 0;
    [SerializeField] bool overhead = false;
    Vector3[] trajectory = new Vector3[] { };
    [SerializeField] float force = 100;
    float reloadTime = 1.0f;
    private float maxDistance;
    Dictionary<float, float> cannonDistanceToTime = new();
    Dictionary<float, float> cannonDistanceToAngle = new();
    private float gravity = 30;
    private void Start()
    {
        cannonDistanceToTime = new();
        maxDistance = Mathf.Pow(force, 2) / gravity;
        bool keepSearching = true;
        float step = 1.0f;
        float currStep = 0.0f;
        float forceSqr = force * force;
        float height = Mathf.Max(transform.position.y, 0);
        while (keepSearching)
        {
            //float angle = Mathf.Asin((gravity * currStep) / (forceSqr)) / 2;
            //if (currStep > (maxDistance * 0.95f))
            //    Debug.Log("Here");
            float main = Mathf.Acos((((gravity * Mathf.Pow(currStep, 2)) / forceSqr) - height) / -Mathf.Pow(Mathf.Pow(height, 2) + Mathf.Pow(currStep, 2), 0.5f));
            float main2 = Mathf.Acos((((gravity * Mathf.Pow(currStep, 2)) / forceSqr) - height) / Mathf.Pow(Mathf.Pow(height, 2) + Mathf.Pow(currStep, 2), 0.5f));
            float raidal = Mathf.Atan(currStep / height);
            float angle = (main + raidal) / 2;
            angle -= 1.5708f;
            float angle2 = (main2 + raidal) / 2;
            //angle %= 1.5708f;
            if (overhead) angle = angle2;// 1.5708f - angle;
            float timeToTarget = currStep / (Mathf.Cos(angle) * force);// 2 * (force / gravity) * Mathf.Sin(angle);//currStep / (Mathf.Cos(angle) * force); //currStep / Mathf.Cos(angle);// 
            cannonDistanceToTime.Add(currStep, timeToTarget);
            cannonDistanceToAngle.Add(currStep, angle);
            currStep += step;
            if (currStep > maxDistance)
                keepSearching = false;
        }
    }
    private float turretDamage = 10;
    public void SetDamage(float damage)
    {
        turretDamage = damage;
    }
    public void SetReloadTime(float time)
    {
        reloadTime = time;
    }
    public Vector3 RequestShot(Vector3 targetPos, out ShootAbility shootAbility)
    {
        Vector3 vDistance = (targetPos - shootPoint.transform.position) * distanceMultiplyer;
        vDistance.y = -vDistance.y;
        Vector3 turn = (new Vector3(targetPos.x, 0, targetPos.z) - new Vector3(shootPoint.transform.position.x, 0, shootPoint.transform.position.z)).normalized;
        float d = Mathf.Pow(Mathf.Pow(vDistance.x, 2) + Mathf.Pow(vDistance.z, 2), 0.5f);
        float angle = GetAngle(gravity, force, d, vDistance.y);
        if (float.IsNaN(angle)) { shootAbility = ShootAbility.toFar; return Vector3.zero; }
        if(!overhead && (angle * Mathf.Rad2Deg) > 30) { shootAbility = ShootAbility.toFar; return Vector3.zero; };
        if (overhead) angle = 1.5708f - angle;
        Vector3 normalDist = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);
        float rotY = Mathf.Atan2(turn.x, turn.z) * Mathf.Rad2Deg;
        normalDist = Rotated(normalDist, 0, rotY - 90, 0, Vector3.down);
        normalDist.y = Mathf.Sin(angle);
        //Debug.DrawLine(shootPoint.transform.position, (normalDist * 5f) + shootPoint.transform.position, Color.red);
        //Debug.Break();
        Vector3 turretCast = normalDist;
        turretCast.y = 0;
        RaycastHit[] hits = (Physics.RaycastAll(turret.transform.position + new Vector3(0, 0.15f, 0), turretCast.normalized, 2.5f));
        foreach (RaycastHit hit in hits)
            if (!hit.collider.CompareTag("IslandOuterCollider") && !hit.collider.CompareTag("SeaTile") && ((hit.collider.gameObject != turret) && (hit.collider.gameObject != barrel) && (hit.collider.gameObject != shootPoint) && (hit.collider.gameObject != turretCube)))
            {
                //Debug.DrawRay(turret.transform.position + new Vector3(0, 0.15f, 0), new Vector3(normalDist.x, 0, normalDist.z), Color.blue, 10f);
                shootAbility = ShootAbility.blocked;
                return Vector3.zero;
            }
        turret.transform.rotation = Quaternion.Euler(0, rotY, 0);
        if (barrelRotationPoint == null)
            barrel.transform.localRotation = Quaternion.Euler(-angle * Mathf.Rad2Deg, 0, 0);
        else
            barrelRotationPoint.transform.localRotation = Quaternion.Euler(-angle * Mathf.Rad2Deg, 0, 0);
        hits = (Physics.RaycastAll(shootPoint.transform.position, normalDist, 2.5f));
        foreach (RaycastHit hit in hits)
            if (!hit.collider.CompareTag("IslandOuterCollider") && !hit.collider.CompareTag("SeaTile") && ((hit.collider.gameObject != turret) && (hit.collider.gameObject != barrel) && (hit.collider.gameObject != shootPoint) && (hit.collider.gameObject != turretCube)))
            {
                shootAbility = ShootAbility.blocked;
                return Vector3.zero;
            }
        //Debug.DrawRay(shootPoint.transform.position, normalDist, Color.red, 5f);
        //Debug.Break();
        //ShootProjectile(normalDist);
        shootAbility = ShootAbility.able;
        return normalDist;
    }
    public enum ShootAbility
    {
        able,
        toFar,
        blocked,
        reloading
    }
    public Vector3 RequestShot(Vector2 targetPos, Vector2 targetVelocity, out ShootAbility shootAbility)
    {
        Vector2 turretPos = new Vector2(transform.position.x, transform.position.z);
        Vector2 oneSecOut = targetPos + targetVelocity;
        float currDst = (Vector2.Distance(targetPos, turretPos) * distanceMultiplyer) + distanceAdd;            //chamge transformpos to vector2
        float futureDst = (Vector2.Distance(oneSecOut, turretPos) * distanceMultiplyer) + distanceAdd;
        float diffInDst = futureDst - currDst;
        //Debug.Log(diffInDst);

        if (futureDst > maxDistance) { shootAbility = ShootAbility.toFar; return Vector3.zero; };

        float closestTime = 0;
        float closestDst = 0;
        float closestDstDiff = -1;
        foreach (KeyValuePair<float, float> dstTime in cannonDistanceToTime)
        {
            float tempTarDst = currDst + (diffInDst * dstTime.Value);
            float tempDiff = Mathf.Abs(tempTarDst - dstTime.Key);
            if ((tempDiff < closestDstDiff) || (closestDstDiff == -1))
            {
                closestDstDiff = tempDiff;
                closestTime = dstTime.Value;
                closestDst = dstTime.Key;
            }
        }
        targetPos = targetPos + (targetVelocity * closestTime);

        RaycastHit[] hits = Physics.RaycastAll(transform.position + new Vector3(0, 0.15f, 0), (new Vector3(targetPos.x, 0, targetPos.y) - transform.position), closestDst);
        //Debug.DrawRay(transform.position + new Vector3(0, 0.15f, 0), (new Vector3(targetPos.x, 0, targetPos.y) - transform.position), Color.red);
        //Debug.DrawLine(transform.position, new Vector3(targetPos.x, 0, targetPos.y), Color.blue);
        foreach (RaycastHit hit in hits)
        {
            //Debug.DrawLine(hit.point, Vector3.zero, Color.green);
            if (hit.collider.CompareTag("Land"))
            {
                Debug.DrawLine(hit.point, Vector3.zero, Color.blue);
                shootAbility = ShootAbility.blocked;
                //Debug.Break();
                return Vector3.zero;
            }
        }
        //float angle = (Mathf.Asin((9.18F * closestDst) / (Mathf.Pow(force, 2)))) / 2;
        //if (overhead) angle = 1.5708f - angle;
        float angle = cannonDistanceToAngle[closestDst] + radAngleAdd;
        if(!overhead && (angle * Mathf.Rad2Deg) > 30) { shootAbility = ShootAbility.toFar; return Vector3.zero; };
        Vector3 turn = (new Vector3(targetPos.x, 0, targetPos.y) - new Vector3(shootPoint.transform.position.x, 0, shootPoint.transform.position.z)).normalized;
        Vector3 normalDist = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);
        float rotY = Mathf.Atan2(turn.x, turn.z) * Mathf.Rad2Deg;
        normalDist = Rotated(normalDist, 0, rotY - 90, 0, Vector3.down);
        normalDist.y = Mathf.Sin(angle);
        Vector3 turretCast = normalDist;
        turretCast.y = 0;
        hits = (Physics.RaycastAll(turret.transform.position + new Vector3(0, 0.15f, 0), turretCast.normalized, 5f));
        foreach (RaycastHit hit in hits)
            if (!hit.collider.CompareTag("IslandOuterCollider") && !hit.collider.CompareTag("SeaTile") && ((hit.collider.gameObject != turret) && (hit.collider.gameObject != barrel) && (hit.collider.gameObject != shootPoint) && (hit.collider.gameObject != turretCube)))
            {
                shootAbility = ShootAbility.blocked;
                return Vector3.zero;
            }

        turret.transform.rotation = Quaternion.Euler(0, rotY, 0);
        if (barrelRotationPoint == null)
            barrel.transform.localRotation = Quaternion.Euler(-angle * Mathf.Rad2Deg, 0, 0);
        else
            barrelRotationPoint.transform.localRotation = Quaternion.Euler(-angle * Mathf.Rad2Deg, 0, 0);

        hits = (Physics.RaycastAll(shootPoint.transform.position, normalDist, 5f));
        foreach (RaycastHit hit in hits)
            if (!hit.collider.CompareTag("IslandOuterCollider") && !hit.collider.CompareTag("SeaTile") && ((hit.collider.gameObject != turret) && (hit.collider.gameObject != barrel) && (hit.collider.gameObject != shootPoint) && (hit.collider.gameObject != turretCube)))
            {
                shootAbility = ShootAbility.blocked;
                return Vector3.zero;
            }
        //ShootProjectile(normalDist);
        shootAbility = ShootAbility.able;
        return normalDist;
    }
    public float ShootProjectile(Vector3 normalDist, int teamId)
    {
        GameObject proj = GameObject.Instantiate(projectile);
        if (shipValues != null)
            proj.GetComponent<ProjectileHit>().shipParts = shipValues.shipParts;
        else if (fortValues != null)
            proj.GetComponent<ProjectileHit>().shipParts = fortValues.fortParts;
        //Debug.Log("Parts: " + proj.GetComponent<ProjectileHit>().shipParts.Count);
        proj.GetComponent<ProjectileHit>().SetDamage(turretDamage);
        proj.GetComponent<ProjectileHit>().SetTeam(teamId);
        proj.transform.position = shootPoint.transform.position;
        proj.GetComponent<Rigidbody>().velocity = normalDist * force;
        //Debug.Break();
        return reloadTime;
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
        float angle = GetAngle(gravity, locForce, d, vDistance.y);
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
            y -= (gravity * (Mathf.Pow(x, 2) / (2 * Mathf.Pow(force, 2) * Mathf.Pow(Mathf.Cos(angle), 2))));
            points[x] = y;
        }

        return points;
    }
    float GetHeightAtTime(float force, float angle, float initialHeight, float time)
    {
        float y = initialHeight + (time * Mathf.Tan(angle));
        y -= (gravity * (Mathf.Pow(time, 2) / (2 * Mathf.Pow(force, 2) * Mathf.Pow(Mathf.Cos(angle), 2))));
        return y;
    }
    public Vector3 Rotated(Vector3 vector, Quaternion rotation, Vector3 pivot)
    {
        return rotation * (vector - pivot) + pivot;
    }

    public Vector3 Rotated(Vector3 vector, Vector3 rotation, Vector3 pivot)
    {
        return Rotated(vector, Quaternion.Euler(rotation), pivot);
    }

    public Vector3 Rotated(Vector3 vector, float x, float y, float z, Vector3 pivot)
    {
        return Rotated(vector, Quaternion.Euler(x, y, z), pivot);
    }
}
