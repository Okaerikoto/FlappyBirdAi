using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public float speedUpGame = 1;
    public float defilementSpeed = 0.75f;
    public float waitBeforeFirstInstantiation = 0f;
    public float InstantiationDelta = 1.5f;
    public bool randomizeTubes = false;
    public int randomizeTubesSeed = 255;
    public float jumpForce = 1.6f;

    private float InstantiationTimer;
    public GameObject tube;
    public GameObject bird;

    

    public bool GameIsOn = false;

    private void Awake()
    {
        Time.timeScale = Mathf.Max(0.0001f, speedUpGame);
    }

    // Start is called before the first frame update
    void Start()
    {
        InstantiationTimer = waitBeforeFirstInstantiation;
        Random.InitState(255);

    }

    // Update is called once per frame
    void Update()
    {
        Time.timeScale = Mathf.Max(0.0001f, speedUpGame);

        if (GameIsOn)
        {
            InstantiationTimer -= Time.deltaTime;
            if (InstantiationTimer <= 0)
            {
                float h = Random.Range(-0.7f, 0.7f);
                GameObject t = Instantiate(tube, new Vector3(2, h, 0), Quaternion.identity);

                InstantiationTimer = InstantiationDelta; // + Random.Range(-InstantiationDelta / 2, InstantiationDelta / 2);
            }
        }

    }

    public void StartGame()
    {
        if (!randomizeTubes)
        {
            Random.InitState(randomizeTubesSeed);
        }

        GameIsOn = true;
    }

    public void StopGame()
    {
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("Tube");
        foreach (GameObject go in gos)
        {
            Destroy(go);
        }

        GameIsOn = false;

        InstantiationTimer = waitBeforeFirstInstantiation;


    }
}
