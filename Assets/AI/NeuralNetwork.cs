using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class NeuralNetwork : MonoBehaviour
{

    public GameController gameController;
    public GeneticAlgo geneticAlgo;
    public Bird bird;

    private int inputLayerSize = 2;
    public int hidenLayerSize = 6;


    public float input_dx;
    public float input_dy;
    public float[,] weights1;
    public float[] weights2;
    public float[] bias1;
    public float[] hidenNode;
    public float output;


    // Start is called before the first frame update
    void Awake()
    {
        gameController = FindObjectOfType<GameController>();
        geneticAlgo = FindObjectOfType<GeneticAlgo>();
        bird = GetComponent<Bird>();

        weights1 = new float[inputLayerSize, hidenLayerSize];
        weights2 = new float[hidenLayerSize];
        bias1 = new float[hidenLayerSize];

        //for observation in editor
        hidenNode = new float[hidenLayerSize];

        SetRandomParameters();
        //SetTestParameters();
    }

    // Update is called once per frame
    public void Think()
    {
        

        if (bird & bird.alife)
        {

            GameObject tube = FindNextTube();
            if (tube != null)
            {
                input_dx = (tube.transform.position.x - transform.position.x) / 3; //(tube.transform.position.x - transform.position.x+1)/4.0f;
                input_dy = (transform.position.y - tube.transform.position.y);//(transform.position.y - tube.transform.position.y + 1)/2.0f;
            }
            else
            {
                input_dx = (3 - transform.position.x)/3;// (3 - transform.position.x + 1) / 4.0f;
                input_dy = transform.position.y; // (transform.position.y - 0 + 1) / 2.0f;
            }

            output = Eval();

            if (output > 0.5)
            {
                bird.Flap();
            }
        }
    }

    public GameObject FindNextTube()
    {
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("Tube");
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach (GameObject go in gos)
        {
            float curDistance = go.transform.position.x - position.x ;
            if (curDistance < distance & curDistance>-0.25f) //0.20 is the tube radius
            {
                closest = go;
                distance = curDistance;
            }
        }
        if (closest)
        {
            //closest.GetComponentInChildren<SpriteRenderer>().color = Color.blue;
        }
       
        return closest;
    }

    void SetRandomParameters()
    {
        for (int i = 0; i < inputLayerSize; i++)
        {
            for (int j = 0; j < hidenLayerSize; j++)
            {
                weights1[i, j] = RandomGene();
            }
        }

        for (int i = 0; i < hidenLayerSize; i++)
        {
            weights2[i] = RandomGene();
            bias1[i] = RandomGene();
        }
    }

    void SetTestParameters()
    {
        for (int i = 0; i < inputLayerSize; i++)
        {
            for (int j = 0; j < hidenLayerSize; j++)
            {
                weights1[i, j] = 1;
            }
        }

        for (int i = 0; i < hidenLayerSize; i++)
        {
            weights2[i] = 1;
            bias1[i] = 0.5f;
        }
    }

    float Eval()
    {      
        float outputLayer = 0;
        for (int j = 0; j < hidenLayerSize; j++)
        {
            float sum = weights1[0, j] * input_dx + weights1[1, j] * input_dy;
            float hidenlayer1out = stepFunction(sum, bias1[j]);
            hidenNode[j] = hidenlayer1out;

            outputLayer += weights2[j] * hidenlayer1out;
        }

        return outputLayer;
    }

    float stepFunction(float input,float tresh)
    {
        if (input > tresh)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }

    //change a bit the weights
    public void Mutate()
    {
        //Mutate all genes
        //for (int i = 0; i < inputLayerSize; i++)
        //{
        //    for (int j = 0; j < hidenLayerSize; j++)
        //    {
        //        weights1[i, j] = weights1[i, j] + Random.Range(-geneticAlgo.evolution_step/2.0f, geneticAlgo.evolution_step / 2.0f);
        //    }
        //}

        for (int i = 0; i < hidenLayerSize; i++)
        {
            weights2[i] = MutateGene(weights2[i]);
            bias1[i] = MutateGene(bias1[i]);
        }
        for (int i = 0; i < inputLayerSize; i++)
        {
            for (int j = 0; j < hidenLayerSize; j++)
            {
                weights1[i, j] = MutateGene(weights1[i, j]);
            }
        }

        for (int i = 0; i < hidenLayerSize; i++)
        {
            weights2[i] = MutateGene(weights2[i]);
            bias1[i] = MutateGene(bias1[i]);
        }
    }

    public float RandomGene()
    {
        float t = geneticAlgo.geneInitialisationValueRange;
        return Random.Range(-t, t);
    }

    public float MutateGene(float gene)
    {
        float r = Random.Range(0.0f, 1.0f);       
        if (r < geneticAlgo.mutation_rate)
        {
            float mutateFactor = 1 + (Random.Range(-geneticAlgo.mutationStrength, geneticAlgo.mutationStrength));
            gene = mutateFactor * gene;
        }
        return gene;
    }

    public void Clone(NeuralNetwork sourceNn, Bird sourceBird)
    {

        bird=sourceBird;
        inputLayerSize = sourceNn.inputLayerSize;
        hidenLayerSize = sourceNn.hidenLayerSize;

        for (int i = 0; i < inputLayerSize; i++)
        {
            for (int j = 0; j < hidenLayerSize; j++)
            {
                weights1[i, j] = sourceNn.weights1[i, j];
            }
        }

        for (int i = 0; i < hidenLayerSize; i++)
        {
            weights2[i] = sourceNn.weights2[i];
            bias1[i] = sourceNn.bias1[i];
        }
    }

    public void Breed(NeuralNetwork motherNn, NeuralNetwork fatherNn, Bird hostBird)
    {

        bird = hostBird;
        inputLayerSize = motherNn.inputLayerSize;
        hidenLayerSize = motherNn.hidenLayerSize;


        for (int i = 0; i < inputLayerSize; i++)
        {
            for (int j = 0; j < hidenLayerSize; j++)
            {

                if (geneticAlgo.breedOnlyBias)
                {
                    weights1[i, j] = motherNn.weights1[i, j];
                }
                else
                {
                    weights1[i, j] = (Random.Range(0, 2) == 0) ? motherNn.weights1[i, j] : fatherNn.weights1[i, j]; 
                }
            }
        }

        for (int i = 0; i < hidenLayerSize; i++)
        {
            if (geneticAlgo.breedOnlyBias)
            {
                weights2[i] = motherNn.weights2[i];
            }
            else
            {
                weights2[i] = (Random.Range(0, 2) == 0) ? motherNn.weights2[i] : fatherNn.weights2[i];
            }
           
            bias1[i] = (Random.Range(0, 2) == 0) ? motherNn.bias1[i] : fatherNn.bias1[i];
        }
    }

    public float RegularisationTerm()
    {
        float r = 0;
        for (int i = 0; i < inputLayerSize; i++)
        {
            for (int j = 0; j < hidenLayerSize; j++)
            {
                r += Mathf.Abs(weights1[i, j]);
            }
        }

        for (int i = 0; i < hidenLayerSize; i++)
        {
            r += Mathf.Abs(weights2[i]);
            r += Mathf.Abs(bias1[i]);
        }

        return geneticAlgo.regularisationWeight * r;
    }



}
