using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{

    PlayerMovement pM;

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
            if (pM.playerID == 1)
            {

                if ( Input.GetKeyDown(KeyCode.E))
                {
                    interactable.Interact(playerID);
                }

            }
            else if (pM.playerID == 2)
            {
                if (Input.GetKeyDown(KeyCode.RightControl))
                {
                    interactable.Interact(playerID);

                }
            }
        }
        
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("interactable"))
        {
            interactable = other.GetComponent<IInteractable>();
            playerInRange = true;
        }

    }
    private void OnTriggerExit(Collider other)
    {

        playerInRange = false;

    }
}
