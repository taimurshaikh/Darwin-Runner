using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FitnessGraphManager : MonoBehaviour {

    private DD_DataDiagram m_DataDiagram;
    GameObject dd;
    GameObject l;
    
    // Use this for initialization
    void Start () {

        dd = GameObject.Find("FitnessGraph");
        if(null == dd) {
            Debug.LogWarning("Can't find Fitness Graph");
            return;
        }
        m_DataDiagram = dd.GetComponent<DD_DataDiagram>();

        l = m_DataDiagram.AddLine("Fitness Graph", Color.red);
    }

    public void AddNewPoint(float x, float y) {    
       m_DataDiagram.InputPoint(l, new Vector2(x, y));
    }

}
