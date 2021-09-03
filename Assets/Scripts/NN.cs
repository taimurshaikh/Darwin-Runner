using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics.LinearAlgebra;
using Random = UnityEngine.Random;

public class NN
{
    public static int numHiddenNodes = 8;
    public static int numHiddenLayers = 3;
    public static int numInputs = 10;
    public static int numOutputs = 5;

    // This is needed for Crossover in GAManager
    public static int numInputWeights = numInputs * numHiddenNodes;
    public static int numHiddenToHiddenWeights = numHiddenNodes * numHiddenNodes * (numHiddenLayers-1);
    public static int numOutputWeights = numHiddenNodes * numOutputs;
    public static int totalWeights = numInputWeights + numHiddenToHiddenWeights + numOutputWeights;

    // 1 x 10 matrix as there are 10 inputs
    public Matrix<float> inputLayer = Matrix<float>.Build.Dense(1, numInputs);
    
    // There are a dynamic number of hidden layers so we need a list of matrices
    public List<Matrix<float>> hiddenLayers = new List<Matrix<float>>();

    // 5 outputs
    public Matrix<float> outputLayer = Matrix<float>.Build.Dense(1, numOutputs);
    
    // Weights between layers
    public List<Matrix<float>> weights = new List<Matrix<float>>();
    
    // List of biases which corresponds to the bias of a layer
    public List<float> biases = new List<float>();

    public void Init()
    {
        // Clear all matrices of garbage values
        inputLayer.Clear();
        hiddenLayers.Clear();
        outputLayer.Clear();
        weights.Clear();
        biases.Clear();
        
        // Populate hidden layers
        PopulateNN();
        RandomiseWeights();
    }

    public void PopulateNN()
    {
        // Handle hidden layers and weights
        for (int i = 0; i < numHiddenLayers; i++)
        {
            Matrix<float> newHiddenLayer = Matrix<float>.Build.Dense(1, numHiddenNodes);
            hiddenLayers.Add(newHiddenLayer);
            biases.Add(Random.Range(-1f, 1f));

            // Matrix for input to first hidden layer will have different dimensions
            if (i == 0)
            {
                Matrix<float> firstWeights = Matrix<float>.Build.Dense(numInputs, numHiddenNodes);
                weights.Add(firstWeights);
            }

            if (i < numHiddenLayers - 1)
            {   
                Matrix<float> newWeights = Matrix<float>.Build.Dense(numHiddenNodes, numHiddenNodes);
                weights.Add(newWeights);
            }
        }
        // Matrix for last hidden to output layer
        Matrix<float> outputWeights = Matrix<float>.Build.Dense(numHiddenNodes, numOutputs);
        weights.Add(outputWeights);
        biases.Add(Random.Range(-1f, 1f));
    }

    public void FeedForward()
    {
        // Activate Input Layer
        inputLayer = inputLayer.PointwiseTanh();

        // Input to first hidden layer
        hiddenLayers[0] = (inputLayer * weights[0]) + biases[0];
        hiddenLayers[0].PointwiseTanh();

        // First to penultimate hidden layer
        for (int i = 1; i < hiddenLayers.Count; i++)
        {
            hiddenLayers[i] = ((hiddenLayers[i-1] * weights[i]) + biases[i]).PointwiseTanh();
        }
        // Last hidden layer to output layer
        outputLayer = ((hiddenLayers[hiddenLayers.Count-1] * weights[weights.Count-1]) + biases[biases.Count-1]);
        for (int i = 0; i < outputLayer.ColumnCount; i++)
        {
            // Sigmoid every element in output vector
            outputLayer[0, i] = Sigmoid(outputLayer[0, i]);
        }
    }

    public void RandomiseWeights()
    {
        for (int i = 0; i < weights.Count; i++)
        {
            for (int j = 0; j < weights[i].RowCount; j++)
            {
                for (int k = 0; k < weights[i].ColumnCount; k++)
                {
                    weights[i][j, k] = Random.Range(-1f, 1f);
                }
            }
        }
    }

    private float Sigmoid(float z)
    {
        return (1 / (1 + Mathf.Exp(-z)));
    }
    
}
