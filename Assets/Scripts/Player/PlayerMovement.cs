using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody rb;
    Animator anim;

    public KeyCode upButton;
    public KeyCode downButton;
    public KeyCode rightButton;
    public KeyCode leftButton;

    public bool isInMenu = false;
    public int playerID;
    public float speed = 10f;
    

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isInMenu)
        {
            Movement();
        }
    }

    void Movement()
    {   
       
            Vector3 move1 = Vector3.zero;

            if (Input.GetKey(upButton))
                move1.y += 1;
            if (Input.GetKey(downButton))
                move1.y -= 1;
            if (Input.GetKey(rightButton))
                move1.x += 1;
            if (Input.GetKey(leftButton))
                move1.x -= 1;
            //Gets the player's movement direction

            move1 = move1.normalized;

            rb.AddForce(move1 * speed, ForceMode.Impulse);
            //Normalizes the vector and adds force



    }
}
