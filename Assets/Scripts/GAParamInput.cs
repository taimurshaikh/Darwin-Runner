using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class GAParamInput : MonoBehaviour
{
    string mutationRate = "";
    string popSize = "";
    public Text mrText;
    public Text psText;
    
    public GameObject mainMenu;

    [HideInInspector]
    public int ps;

    [HideInInspector]
    public float mr;

    GameObject holder;

    void Start()
    {
        holder = GameObject.Find("Holder");
    }
    public void StoreData()
    {   
        mutationRate = mrText.text;
        popSize = psText.text;

        if (!ValidateData(mutationRate, popSize))
        {
            Debug.Log("Invalid input");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else
        {
            ps = int.Parse(popSize);
            mr = float.Parse(mutationRate);
            holder.GetComponent<HoldValues>().Hold();
            mainMenu.GetComponent<MainMenu>().StartVisualisation();
        }
    }

    bool ValidateData(string mr, string ps)
    {
        float mrFlt;
        int psInt;
        try
        {
            mrFlt = float.Parse(mr);
            psInt = int.Parse(ps);
        }
        catch
        {
            return false;
        }

        if (mrFlt < 0f || mrFlt > 1f || psInt < 2 || psInt > 50)
        {
            return false;
        }

        return true;
    }
    // public InputField inputText;

    // // Use this for initialization
    // void Start () {
    //     inputText.onValueChanged.AddListener(delegate {
    //         DebugInput();
    //     });
    // }

    // private void DebugInput(){
    //     Debug.Log ("Input: " + inputText);
    // }
}
