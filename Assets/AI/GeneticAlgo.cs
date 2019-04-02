using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;

/*Notes
 * Do not mutate every genes, only a part
 * mutation = scaling, not traslating 
 */

public class GeneticAlgo : MonoBehaviour
{

    public GameObject birdAi;
    public int playerNumber = 10;
    public int winnersNumber = 2;
    //public float evolution_step = 0.2f;
    public float mutation_rate = 0.2f;
    public float mutationStrength = 2;
    public float bestScore =0;
    public float regularisationWeight = 0;
    public bool breedOnlyBias = false;
    public int generation = 0;
    public bool regenerateWhenTooStupid = false;
    public float geneInitialisationValueRange = 1;

    public GameObject[] runningBirds;
    private GameObject[] oldRunningBirds;
    private List<GameObject> birdsToKill;
    private GameObject[] winningBirds;
    public float[] currentBirdScores;

    

    private GameController gameController;

    private void Awake()
    {
        gameController = FindObjectOfType<GameController>();
    }
    // Start is called before the first frame update
    void Start()
    {
        runningBirds = new GameObject[playerNumber];
        oldRunningBirds = new GameObject[playerNumber];
        birdsToKill = new List<GameObject>();
        winningBirds = new GameObject[playerNumber];
        currentBirdScores = new float[playerNumber];

        GeneratePlayers();
        gameController.StartGame();
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (IsAllPlayerDead())
        {
            gameController.StopGame();
            SelectWinners();
            
            generation += 1;
            Debug.Log("Generation " + generation);
            GenerateNewGenerationPlayer();
            KillLoosingBird();
            gameController.StartGame();
        }

        else
        {
            for (int i = 0; i < playerNumber; i++)
            {
                runningBirds[i].GetComponent<NeuralNetwork>().Think();
            }
        }

        // sort birds
        runningBirds = runningBirds.OrderByDescending(go => go.GetComponent<Bird>().score).ToArray();
        //Selection.activeGameObject = runningBirds[0];

        //display scores
        for (int i = 0; i < playerNumber; i++)
        {
            currentBirdScores[i] = runningBirds[i].GetComponent<Bird>().score;
            if (currentBirdScores[i] > bestScore)
            {
                bestScore = currentBirdScores[i];
            }
        }
    }

    private void Update()
    {
        runningBirds = runningBirds.OrderByDescending(go => go.GetComponent<Bird>().score).ToArray();
    }

    void GeneratePlayers()
    {
        for (int i=0; i< playerNumber; i++)
        {
            runningBirds[i] = Instantiate(birdAi, new Vector3(-1, 0, 0), Quaternion.identity);
            runningBirds[i].name = ("bird " + generation + " " + i).ToString();
        }

        gameController.StartGame();

    }

    void GenerateNewGenerationPlayer()
    {

        ///Keep bests and evolve them
        //for (int i = 0; i < playerNumber; i++)
        //{
        //    if (i < winnersNumber)
        //    {
        //        //Keep the bird 
        //        runningBirds[i].GetComponent<Bird>().Reset();
        //        runningBirds[i].GetComponent<NeuralNetwork>().Evolve();
        //    }
        //    else
        //    {
        //        Destroy(runningBirds[i]);
        //        runningBirds[i] = Instantiate(birdAi, new Vector3(-1, 0, 0), Quaternion.identity);
        //        runningBirds[i].name = ("bird " + generation + " " + i).ToString();
        //    }
        //}

        ///Keep bests and generate all the others from clone evolutions
        //for (int i = 0; i < playerNumber; i++)
        //{
        //    oldRunningBirds[i] = runningBirds[i];

        //    //Pick a parent from the winners
        //    int k = Random.Range(0, winnersNumber);

        //    runningBirds[i] = CloneBird(winningBirds[k]);
        //    runningBirds[i].name = ("bird " + generation + " " + k).ToString();
        //    runningBirds[i].GetComponent<NeuralNetwork>().Evolve();
        //}

        ///Keep bests and breed and evolve children
        for (int i = 0; i < playerNumber; i++)
        {
            if (regenerateWhenTooStupid & runningBirds[0].GetComponent<Bird>().score < 2.6) //=did not reach the first barrier
            {
                KillRunningBirdS();
                GeneratePlayers();
            }

            if (i < winnersNumber)
            {
                //Keep the best birds
                runningBirds[i].GetComponent<Bird>().Reset();
            }
            else if ( i >= winnersNumber & i < 2*winnersNumber)
            {
                birdsToKill.Add(runningBirds[i]);

                //Clone and Evolve the best birds
                runningBirds[i] = CloneBird(runningBirds[i- winnersNumber]);
                runningBirds[i].GetComponent<NeuralNetwork>().Mutate();
                runningBirds[i].name = ("bird " + generation + " " + i).ToString();
            }
            else
            {
                oldRunningBirds[i] = runningBirds[i];
                birdsToKill.Add(runningBirds[i]);

                //Pick parents from the winners
                int j = Random.Range(0, winnersNumber);
                int k = Random.Range(0, winnersNumber);

                //set j the bird with best score
                if (j < k)
                {
                    int t = j;
                    j = k;
                    k = t;
                }

                runningBirds[i] = BreedBird(winningBirds[j], winningBirds[k]);
                runningBirds[i].name = ("bird " + generation + " " + j + "_" + k).ToString();
                runningBirds[i].GetComponent<NeuralNetwork>().Mutate(); 
            }
        }

        gameController.StartGame();
    }

    bool IsAllPlayerDead()
    {
        for (int i = 0; i < playerNumber; i++)
        {
            if (runningBirds[i].GetComponent<Bird>().alife)
            {
                return false;
            }
        }
        return true;
    }

    void SelectWinners()
    {
        runningBirds = runningBirds.OrderByDescending(go => go.GetComponent<Bird>().score).ToArray();

        for (int i = 0; i < winnersNumber; i++)
        {
            winningBirds[i] = runningBirds[i];
        }

    }

    public GameObject CloneBird(GameObject motherBird)
    {
        GameObject babyBird = Instantiate(birdAi, new Vector3(-1, 0, 0), Quaternion.identity);
        babyBird.GetComponent<NeuralNetwork>().Clone(motherBird.GetComponent<NeuralNetwork>(), babyBird.GetComponent<Bird>());
        return babyBird;
    }

    public GameObject BreedBird(GameObject motherBird, GameObject fatherBird)
    {
        GameObject babyBird = Instantiate(birdAi, new Vector3(-1, 0, 0), Quaternion.identity);
        babyBird.GetComponent<NeuralNetwork>().Breed(motherBird.GetComponent<NeuralNetwork>(), fatherBird.GetComponent<NeuralNetwork>(), babyBird.GetComponent<Bird>());
        return babyBird;
    }

    void KillRunningBirdS()
    {
        for (int i = 0; i < playerNumber; i++)
        {
            Destroy(runningBirds[i]);
        }
    }

    void KillLoosingBird()
    {
        foreach (GameObject go in birdsToKill)
        {
            Destroy(go);
        }
    }

    void OnDrawGizmos()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.black;
        Handles.color = Color.black;

        Vector2 dx = new Vector2(0.8f, 0);
        Vector2 dy = new Vector2(0, -0.3f);

        Vector2 pos = new Vector2(0.1f, 0.7f);
        Vector2 pos11 = pos + 1.5f * dy;
        Vector2 pos12 = pos + 3.5f * dy;
        Vector2 pos31 = pos + 2.5f * dy + 2 * dx;


        //GUI.TextField(new Rect(pos11, 0.1f * Vector2.one), "Text");

        NeuralNetwork nn = runningBirds[0].GetComponent<NeuralNetwork>();

        for (int i = 0; i < nn.hidenLayerSize; i++)
        {
            Vector2 pos2i = pos + dx + i * dy;

            DrawConnection(nn.weights1[0, i], pos11, pos2i, -dx / 3 - dy / 5);
            DrawConnection(nn.weights1[1, i], pos12, pos2i, -dy / 5);
            DrawConnection(nn.weights2[i], pos2i, pos31, -dy / 5 - dx / 5);
            //Gizmos.color = Color.blue;
            //Gizmos.DrawLine(pos11, pos2i);
            //Gizmos.DrawLine(pos12, pos2i);
            //Gizmos.DrawLine(pos2i, pos31);


            //Handles.Label((pos11 + pos2i) / 2 -dy/3 - dx/5, weights1[0,i].ToString("F2"));
            //Handles.Label((pos12 + pos2i) / 2 - dy / 5 , weights1[1,i].ToString("F2"));
            //Handles.Label((pos2i + pos31) / 2 - dy / 5 - dx/5, weights2[i].ToString("F2"));
            //GUI.TextField(new Rect((pos11+pos2i)/2 - dy/2, 0.1f*Vector2.one), "Text");

            //Gizmos.DrawSphere(pos2i, 0.1f);
            DrawNode(nn.hidenNode[i], pos2i);
        }
        DrawNode(nn.input_dx, pos11);
        DrawNode(nn.input_dy, pos12);
        DrawNode(nn.output, pos31);
        //Gizmos.DrawSphere(pos11, 0.1f);
        //Gizmos.DrawSphere(pos12, 0.1f);
        //Gizmos.DrawSphere(pos31, 0.1f);
    }

    void DrawConnection(float weight, Vector2 pos1, Vector2 pos2, Vector2 offset)
    {
        Gizmos.color = (weight > 0) ? Color.green : Color.red;
        Gizmos.DrawLine(pos1, pos2);
        Handles.Label((pos1 + pos2) / 2 + offset, weight.ToString("F2"));
    }

    void DrawNode(float value, Vector2 pos)
    {
        Gizmos.color = (value > 0) ? Color.green : Color.red;
        Gizmos.DrawSphere(pos, 0.1f);
        Handles.Label(pos + new Vector2(0, -0.1f), value.ToString("F2"));
    }


    //GameObject Breed(GameObject bird1, GameObject bird2)
    //{
    //    GameObject bird = Instantiate(birdAi, new Vector3(-1, 0, 0), Quaternion.identity);
    //    bird.GetComponent<NeuralNetwork>().CopyNnFrom(bird1.GetComponent<NeuralNetwork>());
    //}
}
