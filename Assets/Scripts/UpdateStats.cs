using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UpdateStats : MonoBehaviour
{
    public TextMeshPro txt;
    GAManager gm;
    void Start()
    {
        gm = FindObjectOfType<GAManager>(); 
        Debug.Log(txt);
        string newText = "lol get fuked";
        txt.text = newText; 
    }

    void Update()
    {
        
    }
}
