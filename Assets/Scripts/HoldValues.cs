using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HoldValues : MonoBehaviour
{
    GAParamInput input;
    [HideInInspector]
    public int ps;
    [HideInInspector]
    public float mr;
    [HideInInspector]
    public bool firstGen = true;

    public int GenerationNum
    { get; set; }
    public static HoldValues instance;
    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        //Debug.Log("HOLDER AWAKENING");
        GenerationNum = 1;
        DontDestroyOnLoad(gameObject);
    }

    public void Hold()
    {
        if (input != null)
        {
            ps = input.ps;
            mr = input.mr;
        }
    }

    void Update()
    {
        // if we are on the menu screen (Scene 0) and the Param Input screen is currently up then initialize input to the reference of the param input script
        if (SceneManager.GetActiveScene().buildIndex == 0 && GameObject.Find("GAParamInput") != null )
        {
            input = GameObject.Find("GAParamInput").GetComponent<GAParamInput>();
        }
    }
}
