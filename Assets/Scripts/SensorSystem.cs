using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics.LinearAlgebra;

public class SensorSystem : MonoBehaviour
{
    [SerializeField]
    // List of objects that will be considered as obstacles (Spikes, Boomboxes, Enemies)
    private GameObject[] obstacles;
    
    // This dictionary will map the tags of each obstacle type to a numerical ID e.g. "Spike" -> 0
    private Dictionary<string, int> tagsToNum = new Dictionary<string, int>();
    
    // Reference to the AgentNN script of the agent that this SensorSystem script is also attached to 
    public AgentNN agentNN;

    // Defining the maximum distance in all directions that can agent can 'see'
    public float maxViewDistance = 100f;

    int cols;

    private void Start()
    {
        // Populating the tagsToNum dictionary by mapping each obstacle type's unique tag to a unique numerical ID 
        int val = 0;
        foreach (GameObject obstacle in obstacles) {
            tagsToNum[obstacle.tag] = val;
            val++;
        } 

        cols = agentNN.nn.inputLayer.ColumnCount;
    }

    private void FixedUpdate()
    {
        handleSensors();
    }

    private Vector3[] instantiateDirVectors()
    {
        // Returns a list of 5 vectors at increasing angles around center of the agent's transform
        Vector3 left = -transform.right;
        Vector3 fLeft = -transform.right + 0.5f*transform.forward;
        Vector3 forward = transform.forward;
        Vector3 fRight = transform.forward + 0.5f*transform.right;
        Vector3 right = transform.right;
        Vector3[] dirVectors = {left, fLeft, forward, fRight, right};
        return dirVectors;
    }

    private void handleSensors()
    {
        Vector3[] dirVectors = instantiateDirVectors();

        // Arrays whose corresponding elements we will pair up and pass into the Agent's NN
        List<float> hitDistances = new List<float>();
        List<int> collidedObjects = new List<int>();
 
        // This ray is what we use to check for collisions
        Ray ray =  new Ray(transform.position, dirVectors[0]);
        
        // The RaycastHit object is what we use to store the collision event
        RaycastHit hit;

        float hitDistanceToAdd;
        int collisionTagToAdd;

        // Check collision for each sensor
        foreach(Vector3 v in dirVectors) {

            // Set the ray object to be in the direction of one of our sensors
            ray.direction = v;

            // Detecting any collision (provided it is within the agent's view distance)
            if (Physics.Raycast(ray, out hit) && hit.distance <= maxViewDistance)
            {   
                // If a ray detected a collision in the current sensor's direction, then there is an object in line with the sensor
                // AND it is within the agent's viewing radius i.e. the agent can 'see' it.
                // We don't want to detect collisions between other agents in the generation OR anything that isn't tagged 
                // (These are things that the agent should not be able to detect in the game e.g. the Camera or Game Manager)
                if (hit.collider.gameObject.tag != "Agent" && hit.collider.gameObject.tag != "Untagged") {
                    
                    // Handle the invisible walls separately so we can more easily see it in the Editor
                    // Sensors that sense walls will be blue, sensors that sense anything else will be green
                    if (hit.collider.gameObject.tag == "LWall" || hit.collider.gameObject.tag == "RWall") {
                        Debug.DrawLine(ray.origin, hit.point, Color.blue);    
                    } else {
                        Debug.DrawLine(ray.origin, hit.point, Color.green);
                    }
                    
                    // Add the distance from the obstacle to the sensor's source (centre of the agent) to the corresponding array
                    hitDistanceToAdd = hit.distance;
                    // And also add the numerical ID of the obstacle type to this array
                    collisionTagToAdd = tagsToNum[hit.collider.gameObject.tag];
                
                } else {

                    // Not detecting an obstacle means we colour the sensor red
                    Debug.DrawLine(ray.origin, hit.point, Color.red);

                    // If sensors did not detect an obstacle, add dummy values to list as we need the order of the sensors to be preserved when passed into the input layer
                    hitDistanceToAdd = -1f;
                    collisionTagToAdd = -1;
                }

            } else {

                // If the sensor didn't detect anything at all, also colour it red
                //Debug.DrawLine(ray.origin, hit.point, Color.red);

                // Again, add dummy values if there was no RayCast event (no collision)
                hitDistanceToAdd = -1f;
                collisionTagToAdd = -1;
            }

            hitDistances.Add(hitDistanceToAdd);
            collidedObjects.Add(collisionTagToAdd);
            
        }

        // After all sensors have been handled, we can use the data gathered to populate the NN's input layer
        linkToInputLayer(hitDistances, collidedObjects);
    }

    // This method sets the input layer of the agent's NN to the values of the sensor system
    private void linkToInputLayer(List<float> hits, List<int> collisions)
    {
        // Matrix which we will replace the current input layer of the NN with
        Matrix<float> newInputs = Matrix<float>.Build.Dense(1, cols);

        int hitInd = 0;
        int colInd = 0;
        int ind1 = 0;
        int ind2 = 1;

        // Populate with sensor vals
        // Iterating through both the hits and collisions array simultaneously
        for (int i = 0; i < cols; i++) {
            // Inputs at even indexes in the input layer are for the hit distances    
            if (i % 2 == 0) {
                newInputs[0, ind1] = hits[hitInd];
                ind1 += 2;
                hitInd++;
            // Inputs at odd indexes in the input layer are for collision tags
            } else {
                newInputs[0, ind2] = (float)collisions[colInd];
                ind2 += 2;
                colInd++;
            }
        }

        // Now we replace the current input layer with this new one we have populated
        agentNN.nn.inputLayer = newInputs;
    }
}
