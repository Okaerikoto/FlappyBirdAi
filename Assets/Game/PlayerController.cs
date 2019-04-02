using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    
    public Bird bird;

    // Start is called before the first frame update
    void Start()
    {
        bird = GetComponent<Bird>(); 
    }

    // Update is called once per frame
    void Update()
    {
        if (bird.alife)
        {
  
            if (Input.GetKeyDown("space"))
            {
                Debug.Log("escape pressed");
                bird.Flap();
            }
        }
    }   


}
