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
    public override void CollectObservations(VectorSensor sensor)
    {
        List<float> inp;
        //if (ena)
            inp = inputs.GetInputs();
        //else
        //    inp = new List<float>() { 0, 0 };
        foreach (float f in inp)
            sensor.AddObservation(f);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float horizontal = actions.ContinuousActions[0];
        if(ena)
            shipMovement.SetControlsNN(horizontal, 1);
    }
    private bool ena = true;
    public void SetEnables(bool enabled)
    {
        ena = enabled;
    }
}
