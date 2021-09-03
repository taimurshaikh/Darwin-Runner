using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public float moveSpeed;
    float slowestAgentSpeed;
    GameObject[] agents;
    Vector3 startPos;
    
    void Start()
    {  
        startPos = transform.position;    
    }

    void Update()
    {
        agents = GameObject.FindGameObjectsWithTag("Agent");
        if (agents.Count() > 0)
        {
            slowestAgentSpeed = agents.Min(agents => agents.GetComponent<PlayerMove>().forwardSpeed);
            transform.Translate(new Vector3(0f, 0f, slowestAgentSpeed * moveSpeed * Time.deltaTime), Space.World);         
        }
    }

    public void ResetPos()
    {
        transform.position = startPos;
    }
}
