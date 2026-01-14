using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{

    PlayerMovement pM;
    public KeyCode interact;

    int playerID;
    private bool playerInRange;
    private IInteractable interactable;

    private void Awake()
    {
        pM = GetComponent<PlayerMovement>();

        playerID = pM.playerID;
    }

    private void Update()
    {
        if (playerInRange)
        {
          
                if ( Input.GetKeyDown(interact))
                {
                    interactable.Interact(gameObject);
                }
            
        }
        
        //Checks if the player is in range of interactable object, and runs the interact script if the player presses the interact key.
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("interactable"))
        {
            interactable = other.GetComponent<IInteractable>();
            playerInRange = true;
        }
        //Checks if the nearby object is interactible, and gets the interactable component if it is.
    }
    private void OnTriggerExit(Collider other)
    {

        playerInRange = false;

    }
}
