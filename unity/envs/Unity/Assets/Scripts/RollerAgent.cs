using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class RollerAgent : Agent
{
    private Rigidbody rBody; // rigid body for agent
    private Rigidbody npcBody;
    private GameObject npc; // rigid body for NPC

    private float w_value; // select the wall position
    public float forceMultiplier = 10; // The movement speed
    public Transform target; // The goal
    public Transform obstacle;

    // start is called before the first frame update
    void Start()
    {
        rBody = GetComponent<Rigidbody>();
        npc = GameObject.Find("NPC");
        npcBody = npc.GetComponent<Rigidbody>();
    }
    
    // Env. configuration 
    public override void OnEpisodeBegin()
    {
        // respawm the door
        w_value = Random.value;
        if (w_value <= 0.25f)
        {
            target.localPosition = new Vector3(Random.value * -6 - 7, 14.42f, -26.62f);
        }
        if (w_value > 0.25f & w_value <= 0.5f)
        {
            target.localPosition = new Vector3(Random.value * -6 - 7, 14.42f, -34.943f);
        }
        if (w_value > 0.5f & w_value <= 0.75f)
        {
            target.localPosition = new Vector3(-14.14f, 14.42f, Random.value*-7-27);
        }
        if (w_value > 0.75f)
        {
            target.localPosition = new Vector3(-5.56f, 14.42f, Random.value * -7 - 27);
        }

        // respawn the elements  (new positions)
        transform.localPosition = new Vector3(Random.value * -6 -7 , 14.42f, Random.value * -7 - 27);
        obstacle.localPosition = new Vector3(Random.value * -6 - 7, 14.42f, Random.value * -7 - 27);
        npc.transform.localPosition = new Vector3(Random.value * -6 - 7, 14.42f, Random.value * -7 - 27);
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
        //rBody.MovePosition(transform.position + dirToGo);
        rBody.AddForce(dirToGo * forceMultiplier,
            ForceMode.VelocityChange);
        //moveNPC();
    }

    public void MoveNPC()
    {
        var dirToGo = Vector3.zero;
        var rotateDir = Vector3.zero;
        var dirToGoForwardAction =0f;
        var rotateDirAction = 0f;
        var dirToGoSideAction = 0f;



        dirToGoForwardAction = Random.value;
        rotateDirAction = Random.value;
        dirToGoSideAction = Random.value;

        if (dirToGoForwardAction <= 0.5)
            dirToGo = 1f * npc.transform.forward;
        else if (dirToGoForwardAction > 0.5)
            dirToGo = -1f * npc.transform.forward;
        if (rotateDirAction <= 0.5)
            rotateDir = npc.transform.up * -1f;
        else if (rotateDirAction > 0.5)
            rotateDir = npc.transform.up * 1f;
        if (dirToGoSideAction <=  0.5)
            dirToGo = -0.6f * npc.transform.right;
        else if (dirToGoSideAction > 0.5)
            dirToGo = 0.6f * npc.transform.right;

        npc.transform.Rotate(rotateDir, Time.fixedDeltaTime * 300f);
        //rBody.MovePosition(transform.position + dirToGo);
        npcBody.AddForce(dirToGo * forceMultiplier,
            ForceMode.VelocityChange);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)

    {
        // move agent
        MoveAgent(actionBuffers.DiscreteActions);
        MoveNPC();
        // calculate the distance between agent and target 
        float distanceToTarget = Vector3.Distance(transform.localPosition, target.localPosition);
        Debug.Log(GetCumulativeReward());
        // Reached target
        //if (distanceToTarget < 1.42f)
        //{
          //  SetReward(1.0f);
            // stop the agent movement
            //rBody.velocity = Vector3.zero;
            //EndEpisode();
        //}
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        if (Input.GetKey(KeyCode.D))
        {
            discreteActionsOut[1] = 2;
            MoveNPC();
        }
        if (Input.GetKey(KeyCode.W))
        {
            discreteActionsOut[0] = 1;
            MoveNPC();
        }
        if (Input.GetKey(KeyCode.A))
        {
            discreteActionsOut[1] = 1;
            MoveNPC();
        }
        if (Input.GetKey(KeyCode.S))
        {
            discreteActionsOut[0] = 2;
            MoveNPC();
        }
    }
    // Trigger target object
    private void OnTriggerEnter(Collider other)
    {
        // When the  agent reached the target (reward and end episode)
        if (other.tag == "Target")
        {
            AddReward(1f);
            Debug.Log(GetCumulativeReward()); // print rewards
            EndEpisode();
            rBody.velocity = Vector3.zero; // stop the agent movement
            npcBody.velocity = Vector3.zero; // stop npc movement
        }
    }
    // Collision with other objects
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.name != "Target")
        {
            Debug.Log(collision.collider.name);
            AddReward(-0.1f);
        }
    }

}

  
