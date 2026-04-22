/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestWorkerBoard : MonoBehaviour, IInteractable
{
    public string woodType; //pine, birch or oak
    public PassiveWoodGain passiveWoodGain;
    GameObject player;
    PlayerMovement playerMove;
    PlayerInteraction playerInteract;

    private bool uiOpen = false;

    public void Interact(GameObject p)
    {
        if (!uiOpen)
        {
            uiOpen = true;
            //open the UI
            if (player != null)
            {
                player = p;
                playerInteract = p.GetComponent<PlayerInteraction>();
                playerMove = p.GetComponent<PlayerMovement>();
            }
            playerMove.isInMenu = true;
        }
        else
        {
            Debug.Log("The menu is already open, so I'm doing nothing!");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (uiOpen)
        {
            if (Input.GetKeyDown(playerMove.upButton))
            {
                //probably does nothing
            }
            if (Input.GetKeyDown(playerMove.downButton))
            {
                //quit the menu
                uiOpen = false;
                playerMove.isInMenu = false;
                Debug.Log("closed the board");
            }
            if (Input.GetKeyDown(playerMove.leftButton))
            {
                passiveWoodGain.DedelegateWorker(woodType);
                Debug.Log("dedelegated worker");
            }
            if (Input.GetKeyDown(playerMove.rightButton))
            {
                passiveWoodGain.DelegateWorker(woodType);
                Debug.Log("delegated a worker");
            }
        }
    }
}




*/