using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Circles_NN_Inputs : MonoBehaviour
{
    [SerializeField] public Transform target;
    [SerializeField] public Transform self;
    [HideInInspector] public float distance;
    public List<float> GetInputs()
    {
        Vector3 forwardDir = self.forward;
        forwardDir.y = 0;
        float dir = Mathf.Asin(self.forward.z / self.forward.magnitude) * Mathf.Rad2Deg;
        if (self.forward.x < 0) dir = (-((90 * Mathf.Sign(dir)) - dir) + (-90 * Mathf.Sign(dir))) * -1;
        Vector3 toVec = (target.position - self.position).normalized;
        toVec.y = 0;
        float to = Mathf.Asin(toVec.z / toVec.magnitude) * Mathf.Rad2Deg;
        if(toVec.x < 0) to = (-((90 * Mathf.Sign(to)) - to) + (-90 * Mathf.Sign(to))) * -1;
        float angleTo = Mathf.DeltaAngle(dir, to);

        Vector3 posDiff = self.position - target.position;
        distance = posDiff.magnitude;
        posDiff = posDiff.normalized;

        List<float> inputs = new()
        {
            distance,
            posDiff.x,
            posDiff.z,
            angleTo,
        };
        return inputs;
    }
}
