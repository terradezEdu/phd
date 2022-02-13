using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowAgent : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject agent;
    private float rotY;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        rotY += agent.transform.rotation.y;
        transform.rotation = Quaternion.Euler(0, rotY, 0);
    }

    private void LateUpdate()
    {
        //transform.position = agent.transform.position + offset;
        
        Debug.Log(rotY);
        //transform.rotation = agent.transform.rotation;
    }
}
