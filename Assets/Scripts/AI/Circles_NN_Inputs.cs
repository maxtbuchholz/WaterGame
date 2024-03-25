using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Circles_NN_Inputs : MonoBehaviour
{
    [SerializeField] public Transform target = null;
    [SerializeField] public Transform self;
    [SerializeField] public ShipMovement shipMovement;
    [HideInInspector] public float distance;
    int teamId = -1;
    float timeBetweenTargetSearch = 10;
    float time = 0;
    public List<float> GetInputs()
    {
        if (teamId == -1) teamId = shipMovement.teamId;
        if (teamId == -1) return new List<float>() {0, 0 };
        time -= Time.deltaTime;
        if ((target == null) || (time <= 0))
        {
            time = timeBetweenTargetSearch + Random.Range(0f, 1f);
            target = FindTargetController.Instance.GetTarget(transform.position, teamId, FindTargetController.targetType.ship);
        }
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
            distance,
            //posDiff.x,
            //posDiff.z,
            angleTo,
        };
        return inputs;
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
