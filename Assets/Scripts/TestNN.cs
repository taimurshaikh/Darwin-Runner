using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics.LinearAlgebra;

public class TestNN : MonoBehaviour
{
    NN test;
    void Start()
    {
        test = new NN();
        test.Init();
    }

    void Update()
    {
        
    }
}
