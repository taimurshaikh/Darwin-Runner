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
    public bool Ended
    { get; set; }
    public static GAManager instance;

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

        Ended = false;
        // GA Structures and Params
        MatingPool = new List<NN>();
        Fitnesses = new List<float>();
        Population = new List<GameObject>();
        //Debug.Log("GA AWAKING");
        nextGen = new List<NN>();
        Initialising = true;

        // Using the holder object that wasn't destroyed from the Menu scene before this to assign user-defined params
        hold = GameObject.FindWithTag("Holder").GetComponent<HoldValues>();
        mutationRate = hold.mr;
        popSize = hold.ps;
        gm = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
    }

    void Start()    
    {
        InitPop(); 
        Debug.Log("lol");
    }

    public void ResetEvolution()
    {
        GenerationNum = 0;
        nextGen.Clear();
        foreach (GameObject a in Population)
        {
            Destroy(a);
        }
        Population.Clear();
        MatingPool.Clear();
        Fitnesses.Clear();
        gm.Restart(true);
    }

    public void EndEvolution()
    {
        GenerationNum = 0;
        nextGen.Clear();
        foreach (GameObject a in Population)
        {
            Destroy(a);
        }
        Population.Clear();
        MatingPool.Clear();
        Fitnesses.Clear();
        Ended = true;
        gm.QuitToMenu();
    }

    private void InitPop()
    {   
        for(int i = 0; i < popSize; i++)
        {
            Population.Add(Instantiate(agentPrefab));
        }
        Initialising = false;
    }

    public float Fitness(Transform agent, float jumpCount, float slideCount)
    {
        float res = agent.position.z;
        if (jumpCount > 0 && slideCount > 0)
        {
            res -= ((1/jumpCount) + (1/slideCount));
        } 
        return res;
    }

    private NN SelectParent()
    {
        // Start at 0
        int index = 0;

        // Pick a random number between 0 and 1
        float r = Random.Range(0f, 1f);

        // Keep subtracting probabilities until you get less than zero
        // Higher probabilities will be more likely to be fixed since they will
        // subtract a larger number towards zero
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

    private (NN, NN) Crossover(NN parentA, NN parentB)
    {
        NN childA = new NN();
        NN childB = new NN();
        childA.Init();
        childB.Init();

        // Perform an alternating crossover with parents A and B
        for (int i = 0; i < parentA.weights.Count; i++)
        {
            if (i % 2 == 0)
            {
                childA.weights[i] = parentA.weights[i];
            }
            else
            {
                childA.weights[i] = parentB.weights[i];
            }
        }

        for (int i = 0; i < parentA.weights.Count; i++)
        {
            if (i % 2 == 0)
            {
                childB.weights[i] = parentB.weights[i];
            }
            else
            {
                childB.weights[i] = parentA.weights[i];
            }
        }

        return (childA, childB);
    }

    private NN Mutate(NN nn)
    {  
        for (int i = 0; i < nn.weights.Count; i++)
        {
            float val = Random.Range(0f, 1f);
            if (val < mutationRate)
            {
                int rowInd = Random.Range(0, nn.weights[i].RowCount);
                int colInd = Random.Range(0, nn.weights[i].ColumnCount);
                nn.weights[i][rowInd, colInd] = Random.Range(-1f, 1f);
            }
        }

        for (int i = 0; i < nn.biases.Count; i++)
        {
            float val = Random.Range(0f, 1f);
            if (val <= mutationRate)
            {
                nn.biases[i] = Random.Range(-1f, 1f);
            }
        }
        return nn;
    }

    // Runs once all of this generation's agents are dead
    public void EndGeneration()
    {
        nextGen.Clear();
        nextGen.Add(MatingPool[MatingPool.Count - 1]);
        int offset = 0;
        if (popSize % 2 == 0)
        {
            offset = 1;
            nextGen.Add(MatingPool[MatingPool.Count - 2]);
        }

        for (int i = 0; i < Mathf.Floor(popSize / 2) - offset; i++)
        {
            NN parentA = SelectParent();
            NN parentB = SelectParent();
            (NN childA, NN childB) = Crossover(parentA, parentB);
            nextGen.Add(Mutate(childA));
            nextGen.Add(Mutate(childB));
        }
        MatingPool.Clear();
        Fitnesses.Clear();
        Population.Clear();
        
        GenerationNum++;

        Initialising = true;
    }

    public void InitNewGen(bool reset=false)
    {
       // Debug.Log("NEW GEN COUNT: " + nextGen.Count.ToString());
        for(int i = 0; i < popSize; i++)
        {
            //Debug.Log("INSTANTIATING AGENT " + i.ToString());
            GameObject newAgent = Instantiate(agentPrefab);
            //Debug.Break();
           // newAgent.transform.position = Vector3.zero;

            // Only update the newAgents with nextGen NNs if we are not resetting the evolution in this case 
            if(!reset)
            {
                newAgent.transform.Find("AgentNN").GetComponent<AgentNN>().nn = nextGen[i];
            }
            Population.Add(newAgent);
        }

        //Debug.Log("NEW POPULATION INSTANTIATED: SIZE " + Population.Count.ToString());

        Initialising = false;
    }

    public void AgentDeath(NN nn, float fitness, GameObject agent)
    {
        MatingPool.Add(nn);
        Fitnesses.Add(fitness);
        Population.Remove(agent);
        Destroy(agent);
    }

    // void GenerateMatingPool()
    // {
    //     AgentNN agentNN;
    //     float totalFitness = 0f;
    //     float currentFitness = 0f;
    //     float numEntries;
    //     MatingPool.Clear();
        
    //     // First, find total fitness in population
    //     foreach (GameObject agent in population)
    //     {
    //         if(agent != null)
    //         {
    //             currentFitness = agent.transform.Find("AgentNN").GetComponent<AgentNN>().Fitness();
    //             totalFitness += currentFitness;
    //         }  
    //     }

    //     // Based on fitness, entries of the same object will get added to the mating pool
    //     // Higher fitness = more entries in pool, and vice versa
    //     foreach(GameObject agent in population)
    //     {
    //         if (agent != null)
    //         {
    //             agentNN = agent.transform.Find("AgentNN").GetComponent<AgentNN>();
    //             numEntries = Mathf.Floor(agentNN.Fitness() / totalFitness);
    //             for (int i = 0; i < numEntries; i++)
    //             {
    //                 MatingPool.Add(agentNN.nn);
    //             }
    //         }
    //     }

    // }

   private void NormalizeFitnesses()
   {
        // Make score exponentially better?
        for (int i = 0; i < Fitnesses.Count; i++) {
            Fitnesses[i] = Mathf.Pow(Fitnesses[i], 2);
        }

        // Add up all the unnormalized fitnesses
        float sum = 0f;
        for (int i = 0; i < Fitnesses.Count; i++) {
            sum += Fitnesses[i];
        }
        // Divide by the sum
        for (int i = 0; i < Fitnesses.Count; i++) {
            Fitnesses[i] /= sum;
        }    
   }

   public float AverageFitness()
   {
       float res = Fitnesses.Sum() / popSize;
       return res;
   }
}
