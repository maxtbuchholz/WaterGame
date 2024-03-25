using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class Around_Drive_Agent : Agent
{
    [SerializeField] public ShipMovement shipMovement;
    [SerializeField] public Avoid_NN_Inputs inputs;
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxis("Horizontal");
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        List<float> inp;
        if (ena)
            inp = inputs.GetInputs();
        else
            inp = new List<float>(new float[20]);
        foreach (float f in inp)
            sensor.AddObservation(f);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float horizontal = actions.ContinuousActions[0] * 3f;
        //Debug.Log(horizontal);
        if(ena)
            shipMovement.SetControlsNN(horizontal, 0.5f);
    }
    private bool ena = true;
    public void SetEnables(bool enabled)
    {
        ena = enabled;
    }
}
