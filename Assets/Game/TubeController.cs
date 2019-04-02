using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TubeController : MonoBehaviour
{

    private float speed = 1;
    private GameController gameController;

    private void Awake()
    {
        gameController = FindObjectOfType<GameController>();

        speed = gameController.defilementSpeed;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (gameController.GameIsOn)
        {
            transform.Translate(new Vector2(-1, 0) * Time.deltaTime * speed);

            if (transform.position.x < -2.5)
            {
                Destroy(this.gameObject);
            }
        }
    }
}
