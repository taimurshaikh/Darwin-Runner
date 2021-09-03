using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics.LinearAlgebra;

public class SensorSystem : MonoBehaviour
{
    [SerializeField]
    private float sensorLength;
    [SerializeField]
    private GameObject[] obstacles;
    private Dictionary<string, int> tagsToNum = new Dictionary<string, int>();

    public AgentNN agentNN;

    int cols;

    void Start()
    {
        int val = 0;
        foreach (GameObject obstacle in obstacles){
            tagsToNum[obstacle.tag] = val;
            val++;
        } 

        cols = agentNN.nn.inputLayer.ColumnCount;
    }

    void FixedUpdate()
    {
        HandleSensors();
    }

    Vector3[] InstantiateDirVectors()
    {
        // Five vectors at increasing angles of 45 degrees clockwise around center of transform
        Vector3 left = -transform.right;
        Vector3 fLeft = -transform.right + 0.5f*transform.forward;
        Vector3 forward = transform.forward;
        Vector3 fRight = transform.forward + 0.5f*transform.right;
        Vector3 right = transform.right;
        Vector3[] dirVectors = {left, fLeft, forward, fRight, right};
        return dirVectors;
    }

    void HandleSensors()
    {

        Vector3[] dirVectors = InstantiateDirVectors();

        // Arrays to pass into NN
        List<float> hitDistances = new List<float>();
        List<int> collidedObjects = new List<int>();
 
        // Initialise sensors by casting Rays out and checking for collision
        Ray ray =  new Ray(transform.position, dirVectors[0]);
        RaycastHit hit;
        foreach(Vector3 v in dirVectors)
        {
            ray.direction = v;

            if (Physics.Raycast(ray, out hit))
            {
                Debug.DrawLine(ray.origin, hit.point, Color.red);
                if (hit.collider.gameObject.tag != "Agent" && hit.collider.gameObject.tag != "Untagged")
                {
                    hitDistances.Add(hit.distance);
                    collidedObjects.Add(tagsToNum[hit.collider.gameObject.tag]);
                    Debug.DrawLine(ray.origin, hit.point, Color.green);
                }
                else
                {
                    // If sensors did not detect an obstacle, add dummy values to list as we need the order of the sensors to be preserved when passed into the input layer
                    hitDistances.Add(-1f);
                    collidedObjects.Add(-1);
                }
            }
            else
            {
                // Again, add dummy values if there was no RayCast event (no collision)
                hitDistances.Add(-1f);
                collidedObjects.Add(-1);
            }
            
        }
        LinkToInputLayer(hitDistances, collidedObjects);
    }

    // This method sets the input layer of the agent's NN to the values of the sensor system
    void LinkToInputLayer(List<float> hits, List<int> collisions)
    {
        Matrix<float> newInputs = Matrix<float>.Build.Dense(1, cols);

        int hitInd = 0;
        int colInd = 0;
        int ind1 = 0;
        int ind2 = 1;

        // Populate with sensor vals
        for (int i = 0; i < cols; i++)
        {   
            if (i % 2 == 0)
            {
                newInputs[0, ind1] = hits[hitInd];
                ind1 += 2;
                hitInd++;
            }
            else
            {
                newInputs[0, ind2] = (float)collisions[colInd];
                ind2 += 2;
                colInd++;
            }
        }

        agentNN.nn.inputLayer = newInputs;
        // Debug.Log(newInputs);
    }
}
