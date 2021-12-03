using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics.LinearAlgebra;

// This script represents an instance of a relationship between ONE NN and ONE Agent
public class AgentNN : MonoBehaviour
{
    public NN nn;
    public Transform agent;
    public PlayerMove movement;

    // Keeping track of jump and slide counts to use in fitness calculation
    public int JumpCount
    { get; set; } 

    public int SlideCount
    { get; set; }

    // These are two ensure that the jumpCount doesn't get incremented too many times per frame
    float jumpCoolDown = 10f;
    float jumpCoolDownTimer;

    void Awake()
    {
        nn = new NN();
        nn.Init();
    }

    void Update()
    {
        if (jumpCoolDownTimer > 0) {
            jumpCoolDownTimer -= Time.deltaTime;
        }
        else if (jumpCoolDownTimer < 0) {
            jumpCoolDownTimer = 0f;
        }

        nn.FeedForward();
        float maxOutput = 0f;
        int maxOutputInd = 0;
        
        // Find the index of the output node with the maximum value
        // (i.e. the move that the NN is most confident to take)
        for (int i = 0; i < nn.outputLayer.ColumnCount; i++) {
            if (nn.outputLayer[0, i] > maxOutput) {
                maxOutput = nn.outputLayer[0, i];
                maxOutputInd = i;
            }
        }

        // Choose max output and perform the move corresponding to that output node
        // (Which moves correspond to which output node was chosen arbitrarily)
        switch(maxOutputInd) {
            // LEFT (A key)
            case 0:
                movement.horizontal = -1f;
                movement.isJumpPressed = false;
                movement.isSliding = false;
                movement.slideStopped = true;
                break;

            // RIGHT (D key)
            case 1:
                movement.horizontal = 1f;
                movement.isJumpPressed = false;
                movement.isSliding = false;
                movement.slideStopped = true;
                break;

            // DO NOTHING
            case 2:
                movement.horizontal = 0f;
                movement.isJumpPressed = false;
                movement.isSliding = false;
                movement.slideStopped = true;
                break;

            // JUMP (Spacebar)
            case 3:
                if (jumpCoolDownTimer == 0) {  
                    movement.horizontal = 0f;
                    movement.isJumpPressed = true;
                    movement.isSliding = false;
                    movement.slideStopped = true;

                    jumpCoolDownTimer = jumpCoolDown;   

                    JumpCount++;
                }
                break;

            // SLIDE (S key)
            case 4:
                movement.horizontal = 0f;
                movement.isSliding = true;
                movement.isJumpPressed = false;

                SlideCount++;
                break;
        }   
    }
    
}
