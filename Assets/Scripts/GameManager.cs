using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{ 
    public GAManager genetic;
    public HoldValues hold;    
    public static GameManager instance;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        
        //Cursor.lockState = CursorLockMode.Locked;
        hold = GameObject.Find("Holder").GetComponent<HoldValues>();
    }

    void LateUpdate()
    {
        CheckForRestart();
    }

    void CheckForRestart()
    {
        if (genetic.Population.Count == 0 && genetic.Initialising == false)
        {
           // Debug.Log("ENDING GENERATION");
            genetic.EndGeneration();
            Restart();
        }
    }

    void Restart ()
    {
        if (hold.firstGen == true)
        {
            hold.firstGen = false;
        }
       // Debug.Log("NEW SCENE LOADING");
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        //Debug.Log("LOADED");
        GameObject.Find("CourseManager").GetComponent<CourseManager>().SpawnFirstTiles();
        genetic.InitNewGen();
        GameObject.Find("Main Camera").GetComponent<FollowPlayer>().ResetPos();
    }
}
