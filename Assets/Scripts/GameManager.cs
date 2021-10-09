using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{ 
    private GAManager genetic;
    private WindowGraph graph;
    private HoldValues hold;    
    public static GameManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);

        genetic = GameObject.Find("GAManager").GetComponent<GAManager>();
        hold = GameObject.Find("Holder").GetComponent<HoldValues>();
        graph = GameObject.Find("WindowGraph").GetComponent<WindowGraph>();
    }

    void LateUpdate()
    {
        // Checking if current generation has ended
        if (genetic.Population.Count == 0 && genetic.Initialising == false && !genetic.Ended)
        {     
            Restart();
        }
    }

    public void Restart (bool resetEvolution=false)
    {
        genetic.EndGeneration();
        if (hold.firstGen == true && !resetEvolution)
        {
            hold.firstGen = false;
        }
        graph.ClearGraph();
        graph.AddNewPoint(genetic.AverageFitness());
        graph.ShowGraph();
        GameObject.Find("CourseManager").GetComponent<CourseManager>().SpawnFirstTiles();
        genetic.InitNewGen(resetEvolution);
        GameObject.Find("Main Camera").GetComponent<FollowPlayer>().ResetPos();
    }

    public void QuitToMenu()
    {
        hold.firstGen = true;
        SceneManager.LoadScene("StartMenu");
    }
}
