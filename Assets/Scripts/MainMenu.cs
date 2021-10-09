using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject options;
    public GameObject paramInput;
    
    public void Start()
    {
        gameObject.SetActive(true);
        options.SetActive(false);
        paramInput.SetActive(false);    
    }
    
    // Executed when Start Button clicked
    public void StartVisualisation()
    {
        // Loads next Scene in the queue (this is the main visualisation scene as there are only two scenes)
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

        //Cursor.lockState = CursorLockMode.Locked;
    }

    // Executed when Quit Button Clicked
    public void Quit()
    {
        // Application only quits in an actual build of the project, NOT in the Unity Editor, so this debug log is to verify the function executes on quit button click
        Debug.Log("QUIT");
        Application.Quit();
    }
}
