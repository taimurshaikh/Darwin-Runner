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

    private void Awake()
    {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);

        if (SceneManager.GetActiveScene().buildIndex == 0) {
            Destroy(gameObject);
        } else {
            genetic = GameObject.Find("GAManager").GetComponent<GAManager>();
            hold = GameObject.Find("Holder").GetComponent<HoldValues>();
            graph = GameObject.Find("WindowGraph").GetComponent<WindowGraph>();
        }
    }

    private void LateUpdate()
    {
        checkForRestart();
    }

    private void checkForRestart()
    {
        // Checking if current generation has ended
        if (genetic.Population.Count == 0 && genetic.Initialising == false && !genetic.GameEnded) {
            graph.AddNewPoint(genetic.AverageFitness());
            genetic.EndGeneration();     
            Restart();
        }
    }

    public void Restart(bool resetEvolution=false)
    {
        if (hold.firstGen == true && !resetEvolution) {
            hold.firstGen = false;
        }
        
        graph.ClearGraph(resetEvolution);
        graph.ShowGraph();
        GameObject.Find("CourseManager").GetComponent<CourseManager>().SpawnFirstTiles();
        genetic.InitNewGen(resetEvolution);
        GameObject.Find("Main Camera").GetComponent<FollowPlayer>().ResetPos();
    }

    public void QuitToMenu()
    {
        SceneManager.LoadScene("StartMenu");
        Destroy(gameObject);
    }
}
