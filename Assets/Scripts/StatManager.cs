using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatManager : MonoBehaviour
{
    public Text text;
    private List<float> allFitnesses;
    HoldValues hold;
    void Start()
    { 
        hold = GameObject.Find("Holder").GetComponent<HoldValues>();
        allFitnesses = new List<float>();
    }

    void Update()
    {
        text.text  = $"Pop Size: {hold.ps}\nMutation Rate: {hold.mr}\n\nCurrent Gen: {hold.GenerationNum}\nAgents Remaining: {GameObject.FindGameObjectsWithTag("Agent").Length}\nMax Fitness: {FindMaxFitness()}";
    }

    int FindMaxFitness()
    {
        return 1;
    }
}
