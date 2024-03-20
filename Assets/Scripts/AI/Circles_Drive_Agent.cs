using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class Circles_Drive_Agent : Agent
{
    [SerializeField] public ShipMovement shipMovement;
    [SerializeField] public Circles_NN_Inputs inputs;
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxis("Horizontal");
    }
    float goalDistance = 20;
    public override void CollectObservations(VectorSensor sensor)
    {
        List<float> inp = inputs.GetInputs();
        foreach (float f in inp)
            sensor.AddObservation(f);

        float currDst = Mathf.Abs(inputs.distance - goalDistance);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float horizontal = actions.ContinuousActions[0];
        shipMovement.SetControlsNN(horizontal, 1);
    }
}
