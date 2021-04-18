using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour
{
    public float jumpForce = 4.1f;
    public NeuralNetwork nn;
    public bool alife = true;
    public float score = 0;
    private Rigidbody2D rb;
    private GameController gc;

    // Start is called before the first frame update
    void Start()
    {
        gc = GetComponent<GameController>();
        nn = GetComponent<NeuralNetwork>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (transform.position.y > 1.5 | transform.position.y < -1.5)
        {
            alife = false;
        }

        if (alife)
        {
            
            GeneticAlgo ga = FindObjectOfType<GeneticAlgo>();
            if (ga!=null & ga.regularisationWeight != 0 & nn)
            {
                //if (ga.regularisationWeight!=0 & nn)               
                //{
                //    score += Time.deltaTime - nn.RegularisationTerm();
                //} 
            }
            else
            {
                score += Time.deltaTime;
            }
        }
    }

    public void Flap()
    {

        if (alife)
        {
            Debug.Log(name + " is flapping");
            //GetComponent<Rigidbody2D>().AddForce(new Vector2(0, jumpForce));

            rb.velocity = new Vector2(rb.velocity.x, jumpForce);


        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {

        Debug.Log("collision");
        alife = false;
        GetComponent<SpriteRenderer>().enabled = false;
        this.GetComponent<Rigidbody2D>().gravityScale = 0f;
    }

    public void Reset()
    {
        transform.position = new Vector3(-1, 0, 0);
        transform.rotation = Quaternion.identity;    
        score = 0;
        this.GetComponent<Rigidbody2D>().gravityScale = 1f;
        GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        GetComponent<Rigidbody2D>().angularVelocity = 0;
        GetComponent<SpriteRenderer>().enabled = true;
        alife = true;
    }
}
