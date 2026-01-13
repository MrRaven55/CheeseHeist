using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{

    [SerializeField]
    GameObject player1;

    [SerializeField]
    GameObject player2;

    public int camID;

    private Vector3 offset = new Vector3(0, 0, -10);
    private Vector3 velocity = Vector3.zero;
    private float smoothTime = 0.25f;


    private void Awake()
    {
        player1.GetComponent<Transform>();
        player2.GetComponent<Transform>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        CameraFollow();
    }

    void CameraFollow()
    {
        if(camID == 1)
        {
            Vector3 targetPosition = player1.transform.position + offset;
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);

        } 
        else if(camID == 2)
        {
            Vector3 targetPosition = player2.transform.position + offset;
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        }

        //Smoothly follows the player character the camera is assigned to based on the cam ID
    }

}
