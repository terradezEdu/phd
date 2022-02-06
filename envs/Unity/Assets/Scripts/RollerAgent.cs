using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class RollerAgent : Agent
{
    Rigidbody rBody; // rigid body for agent
    private float w_value; // select the wall position
    public float forceMultiplier = 10; // The movement speed
    public Transform Target; // The goal

    // Start is called before the first frame update
    void Start()
    {
        rBody = GetComponent<Rigidbody>();
    }
    
    // Env. configuration 
    public override void OnEpisodeBegin()
    {
        // If the Agent fell, restart the env
        if (this.transform.localPosition.y <0)
        {
            rBody.angularVelocity = Vector3.zero;
            rBody.velocity = Vector3.zero;
            transform.localPosition = new Vector3(0, 0.5f, 0);
        }

        // select the wall to respawm the door
        w_value = Random.value;
        if (w_value <= 0.25f)
        {
            Target.localPosition = new Vector3(Random.value * -6 - 7, 14.42f, -26.62f);
        }
        if (w_value > 0.25f & w_value <= 0.5f)
        {
            Target.localPosition = new Vector3(Random.value * -6 - 7, 14.42f, -34.943f);
        }
        if (w_value > 0.5f & w_value <= 0.75f)
        {
            Target.localPosition = new Vector3(-14.14f, 14.42f, Random.value*-7-27);
        }
        if (w_value > 0.75f)
        {
            Target.localPosition = new Vector3(-5.56f, 14.42f, Random.value * -7 - 27);
        }
    }
    // Update is called once per frame
    void Update()
    {

    }

    public void MoveAgent(ActionSegment<int> act)
    {
        var dirToGo = Vector3.zero;
        var rotateDir = Vector3.zero;
        var dirToGoForwardAction = act[0];
        var rotateDirAction = act[1];
        var dirToGoSideAction = act[2];

        if (dirToGoForwardAction == 1)
            dirToGo =  1f * transform.forward;
        else if (dirToGoForwardAction == 2)
            dirToGo = -1f * transform.forward;
        if (rotateDirAction == 1)
            rotateDir = transform.up * -1f;
        else if (rotateDirAction == 2)
            rotateDir = transform.up * 1f;
        if (dirToGoSideAction == 1)
            dirToGo =  -0.6f * transform.right;
        else if (dirToGoSideAction == 2)
            dirToGo =  0.6f * transform.right;

        transform.Rotate(rotateDir, Time.fixedDeltaTime * 300f);
        rBody.AddForce(dirToGo);
    }


    public override void OnActionReceived(ActionBuffers actionBuffers)

    {
        // move agent
        MoveAgent(actionBuffers.DiscreteActions);

        // Rewards
        float distanceToTarget = Vector3.Distance(transform.localPosition, Target.localPosition);
        
        // Reached target
        if (distanceToTarget < 1.42f)
        {
            SetReward(1.0f);
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        if (Input.GetKey(KeyCode.D))
        {
            discreteActionsOut[1] = 2;
        }
        if (Input.GetKey(KeyCode.W))
        {
            discreteActionsOut[0] = 1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            discreteActionsOut[1] = 1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            discreteActionsOut[0] = 2;
        }
    }
    
}

  
