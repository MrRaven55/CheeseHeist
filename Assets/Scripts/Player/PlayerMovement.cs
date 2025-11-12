using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody rb;
    public int playerID;

    public float maxSpeed = 10f;
    public float speed = 10f;
    

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Movement();
    }

    void Movement()
    {   
        if(playerID == 1)
        {
            Vector3 move1 = Vector3.zero;

            if (Input.GetKey(KeyCode.W))
                move1.y += 1;
            if (Input.GetKey(KeyCode.S))
                move1.y -= 1;
            if (Input.GetKey(KeyCode.D))
                move1.x += 1;
            if (Input.GetKey(KeyCode.A))
                move1.x -= 1;

            move1 = move1.normalized;

            rb.AddForce(move1 * speed, ForceMode.Impulse);


        }
        else if(playerID == 2)
        {
            Vector3 move2 = Vector3.zero;

            if (Input.GetKey(KeyCode.UpArrow))
                move2.y += 1;
            if (Input.GetKey(KeyCode.DownArrow))
                move2.y -= 1;
            if (Input.GetKey(KeyCode.RightArrow))
                move2.x += 1;
            if (Input.GetKey(KeyCode.LeftArrow))
                move2.x -= 1;

            move2 = move2.normalized;

            rb.AddForce(move2 * speed, ForceMode.Impulse);

        }



    }
}
