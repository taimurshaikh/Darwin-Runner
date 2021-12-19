using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MathNet.Numerics.LinearAlgebra;
using Random = UnityEngine.Random;

public class GAManager : MonoBehaviour
{
    HoldValues hold;
    public GameObject agentPrefab;
    GameManager gm;
    public int GenerationNum
    { get; set; }

    [Header("Parameters")]

    public int popSize;
    public float mutationRate;

    public List<GameObject> Population
    { get; set; }
    
    // These two ordered lists correspond with each other - the ith nn in the mating pool has the fitness at the ith position of the fitness list
    public List<NN> MatingPool
    { get; set; }
    public List<float> Fitnesses
    { get; set; }

    private List<NN> nextGen;

    public bool Initialising
    { get; set; }
    public bool GameEnded
    { get; set; }
    public static GAManager instance;

    const float fitnessMult = 0.01f;

    public float maxFitness = 0f;
    void Awake()
    {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

        GameEnded = false;

        // GA Structures and Params
        MatingPool = new List<NN>();
        Fitnesses = new List<float>();
        Population = new List<GameObject>();
        nextGen = new List<NN>();
        Initialising = true;

        // Using the holder object that wasn't destroyed from the Menu scene before this to assign user-defined params
        hold = GameObject.FindWithTag("Holder").GetComponent<HoldValues>();
        mutationRate = hold.MR;
        // TEST 14
        // Debug.Log($"True Mutation Rate: {mutationRate}");

        popSize = hold.PS;
        gm = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();

        GenerationNum = 1;
    }

    void Start()    
    {
        initPop(); 
    }

    private void Update()
    {
        if (GenerationNum < 1) {
            GenerationNum = 1;
        }
    }
    
    private void initPop()
    {   
        for (int i = 0; i < popSize; i++) {
            Population.Add(Instantiate(agentPrefab));
        }
        Initialising = false;
    }

    public float Fitness(Transform agent, float jumpCount, float slideCount)
    {
        float res = agent.position.z;
        if (jumpCount > 0 && slideCount > 0) {
            res -= (fitnessMult * (jumpCount + slideCount));
        } 
        if (res > maxFitness) {
            maxFitness = res;
        }
        return res;
    }

    private NN selectParent()
    {
        // Start at 0
        int index = 0;

        // Pick a random number between 0 and 1
        float r = Random.Range(0f, 1f);

        // Keep subtracting probabilities until you get less than zero
        // Higher probabilities will be more likely to be selected since they will
        // Subtract a larger number towards zero
        while (r > 0) {
            r -= Fitnesses[index];
            // And move on to the next
            index++;
        }

        // Go back one
        index--;
 
        NN res = MatingPool[index];

        return res;
    }

    private (NN, NN) crossover(NN parentA, NN parentB)
    {
        NN childA = new NN();
        NN childB = new NN();
        childA.Init();
        childB.Init();

        // Perform an alternating crossover with parents A and B
        for (int i = 0; i < parentA.weights.Count; i++) {
            if (i % 2 == 0) {
                childA.weights[i] = parentA.weights[i];
            } else {
                childA.weights[i] = parentB.weights[i];
            }
        }

        for (int i = 0; i < parentA.weights.Count; i++) {
            if (i % 2 == 0) {
                childB.weights[i] = parentB.weights[i];
            } else {
                childB.weights[i] = parentA.weights[i];
            }
        }
        return (childA, childB);
    }

    private NN mutate(NN nn)
    {  
        for (int i = 0; i < nn.weights.Count; i++) {
            float val = Random.Range(0f, 1f);
            if (val < mutationRate) {
                int rowInd = Random.Range(0, nn.weights[i].RowCount);
                int colInd = Random.Range(0, nn.weights[i].ColumnCount);
                nn.weights[i][rowInd, colInd] = Random.Range(-1f, 1f);
            }
        }

        for (int i = 0; i < nn.biases.Count; i++) {
            float val = Random.Range(0f, 1f);
            if (val <= mutationRate) {
                nn.biases[i] = Random.Range(-1f, 1f);
            }
        }
        return nn;
    }

    // Runs once all of this generation's agents are dead
    public void EndGeneration()
    {
        nextGen.Clear();
        NN elitism1 = MatingPool[MatingPool.Count - 1];
        NN elitism2 = MatingPool[MatingPool.Count - 1];
        nextGen.Add(elitism1);
        nextGen.Add(elitism2);
        Fitnesses = normalizeFitnesses();
        int offset = 0;
        if (MatingPool.Count % 2 != 0) {
            offset = 1;
        }
        // TEST 26
            // Debug.Log("");
            // Debug.Log("NEW GEN");
            // foreach (float f in Fitnesses)
            // {
            //     Debug.Log(f);
            // }
        for (int i = 0; i < Mathf.Floor(popSize / 2) + offset; i++) {
            NN parentA = selectParent();
            NN parentB = selectParent();

            // TEST 29
            // if (parentA == elitism1 && parentB == elitism2) {
            //     Debug.Log("BOTH");
            // } else if (parentA == elitism1 || parentB == elitism2) {
            //     Debug.Log("ONE");
            // } else {
            //     Debug.Log("NEITHER");
            // }
        
            // TEST 30
            if (parentA.weights == parentB.weights) {
               Debug.Log("TRUE");
            } 
            (NN childA, NN childB) = crossover(parentA, parentB);
            // TEST 31
            // Debug.Log("PARENT A");
            // for (int j = 0; j < parentA.weights.Count(); j++) {
            //     if (j % 2 == 0) {
            //         Debug.Log(parentA.weights[j]);
            //     }
            // }
            // Debug.Log("CHILD A");
            // for (int j = 0; j < childA.weights.Count(); j++) {
            //     if (j % 2 == 0) {
            //         Debug.Log(childA.weights[j]);
            //     }
            // }
            // Debug.Log("PARENT B");
            // for (int j = 0; j < parentB.weights.Count(); j++) {
            //     if (j % 2 != 0) {
            //         Debug.Log(parentB.weights[j]);
            //     }
            // }
            // Debug.Log("CHILD B");
            // for (int j = 0; j < childB.weights.Count(); j++) {
            //      if (j % 2 != 0) {
            //         Debug.Log(childB.weights[j]);
            //     }
            // }
            nextGen.Add(mutate(childA));
            nextGen.Add(mutate(childB));
        }

        MatingPool.Clear();
        Fitnesses.Clear();
        Population.Clear();
        
        GenerationNum++;
        // TEST 12
        // Debug.Log($"True Gen Num: {GenerationNum}");
        Initialising = true;
    }

    public void InitNewGen(bool reset=false)
    {
        for(int i = 0; i < popSize; i++) {
            GameObject newAgent = Instantiate(agentPrefab);

            // Only update the newAgents with nextGen NNs if we are not resetting the evolution in this case 
            if(!reset) {
                newAgent.transform.Find("AgentNN").GetComponent<AgentNN>().nn = nextGen[i];
            }
            Population.Add(newAgent);
        }

        Initialising = false;
    }

    public void AgentDeath(NN nn, float fitness, GameObject agent)
    {
        MatingPool.Add(nn);
        Fitnesses.Add(fitness);
        Population.Remove(agent);
        Destroy(agent);
    }

    public float AverageFitness()
    {
        float avg = Fitnesses.Sum() / popSize; 
        // TEST 11
        // Debug.Log($"Average Fitness for Gen {GenerationNum}: {avg}");
        return avg;
    }

    private List<float> normalizeFitnesses()
    {
        float minFitness = Fitnesses.Min();
        float maxFitness = Fitnesses.Max();
        float deltaF = maxFitness - minFitness; 
        List<float> normalizedFitnesses = new List<float>();
        foreach (float fitness in Fitnesses) {
            normalizedFitnesses.Add((fitness - minFitness) / deltaF);
        }
        // TEST 37
        // foreach (float f in Fitnesses)
        // {
        //     Debug.Log(f);
        // }
        // foreach (float f in normalizedFitnesses)
        // {
        //     Debug.Log(f);
        // }
        return normalizedFitnesses;
    }

    private void clearGAParams() 
    {
        GenerationNum = 0;
        nextGen.Clear();
        foreach (GameObject a in Population) {
            Destroy(a);
        }
        Population.Clear();
        MatingPool.Clear();
        Fitnesses.Clear();
    }

    public void ResetEvolution()
    {
        clearGAParams();
        gm.Restart(true);
    }

    public void EndEvolution()
    {
        clearGAParams();
        GameEnded = true;
        gm.QuitToMenu();
        Destroy(gameObject);
    }
}
