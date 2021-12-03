using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class StatManager : MonoBehaviour
{
    public Text text;
    HoldValues hold;
    GAManager genetic;
    void Start()
    { 
        hold = GameObject.Find("Holder").GetComponent<HoldValues>();
        genetic = GameObject.Find("GAManager").GetComponent<GAManager>();
    }

    void Update()
    {
        text.text  = $"Pop Size: {hold.PS}\nMutation Rate: {hold.MR}\n\nCurrent Gen: {genetic.GenerationNum}\nAgents Remaining: {GameObject.FindGameObjectsWithTag("Agent").Length}\nMax Fitness: {(genetic.Fitnesses.Count > 0 ? genetic.Fitnesses.Max(): 0f)}";
    }
}
