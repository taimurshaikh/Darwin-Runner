using UnityEngine;
using UnityEngine.UI;
using System;
public class Score : MonoBehaviour
{
    public Text scoreText;
    public Transform player;
    public float offset = 3f;

    void Update()
    {
        float temp = (player.position.z / 5) + offset;
        scoreText.text = temp.ToString("0"); 
    
    }
}
