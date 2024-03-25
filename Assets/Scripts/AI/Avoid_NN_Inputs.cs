using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Avoid_NN_Inputs : MonoBehaviour
{
    [SerializeField] public Transform target;
    [SerializeField] public Transform self;
    [HideInInspector] public float distance;
    private void Start()
    {
        
    }
    public List<float> GetInputs()
    {
        if (target == null) target = PointToPlayer.Instance.GetPlayerShip();
        Vector3 aimPose = GetAimPos(target.position);
        Vector3 forwardDir = self.forward;
        forwardDir.y = 0;
        float dir = Mathf.Asin(self.forward.z / self.forward.magnitude) * Mathf.Rad2Deg;
        if (self.forward.x < 0) dir = (-((90 * Mathf.Sign(dir)) - dir) + (-90 * Mathf.Sign(dir))) * -1;
        Vector3 toVec = (aimPose - self.position).normalized;
        toVec.y = 0;
        float to = Mathf.Asin(toVec.z / toVec.magnitude) * Mathf.Rad2Deg;
        if(toVec.x < 0) to = (-((90 * Mathf.Sign(to)) - to) + (-90 * Mathf.Sign(to))) * -1;
        float angleTo = Mathf.DeltaAngle(dir, to);

        Vector3 posDiff = self.position - aimPose;
        distance = posDiff.magnitude;
        posDiff = posDiff.normalized;

        List<float> inputs = new()
        {
            //distance,
            ////posDiff.x,
            ////posDiff.z,
            //angleTo,1
            GetDistanceInDirection(0,50),
            GetDistanceInDirection(10,50),
            GetDistanceInDirection(-10,50),
            GetDistanceInDirection(20,50),
            GetDistanceInDirection(40,50),
            GetDistanceInDirection(60,50),
            GetDistanceInDirection(80,50),
            GetDistanceInDirection(100,50),
            GetDistanceInDirection(120,50),
            GetDistanceInDirection(140,50),
            GetDistanceInDirection(160,50),
            GetDistanceInDirection(180,50),
            GetDistanceInDirection(-20,50),
            GetDistanceInDirection(-40,50),
            GetDistanceInDirection(-60,50),
            GetDistanceInDirection(-80,50),
            GetDistanceInDirection(-100,50),
            GetDistanceInDirection(-120,50),
            GetDistanceInDirection(-140,50),
            GetDistanceInDirection(-160,50),
        };
        return inputs;
    }
    private float GetDistanceInDirection(float angle, float distance)
    {
        RaycastHit[] hits = Physics.RaycastAll(transform.position, (Quaternion.Euler(0, angle, 0) * transform.forward), distance);
        float groundHitDistance = -1;
        foreach(RaycastHit hit in hits)
        {
            if (hit.collider.CompareTag("Land"))
                if ((groundHitDistance == -1) || (hit.distance < groundHitDistance))
                    groundHitDistance = hit.distance;
        }
        return groundHitDistance;
    }
    private Vector3 GetAimPos(Vector3 target)
    {
        Vector3 dir = target - transform.position;
        float mag = dir.magnitude;
        dir.y = 0;
        dir = dir.normalized;
        mag -= 20;
        dir *= mag;
        return transform.position + dir;
    }
}
