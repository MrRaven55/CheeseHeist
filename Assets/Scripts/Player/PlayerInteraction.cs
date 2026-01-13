using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{

    PlayerMovement pM;

    int playerID;
    private float timerSet;
    private float timer;
    private bool playerInRange;
    private Collider collider;
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

                if (collider.gameObject.CompareTag("interactable") && Input.GetKeyDown(KeyCode.E))
                {
                    interactable.Interact(playerID);
                }

            }
            else if (pM.playerID == 2)
            {
                if (collider.gameObject.CompareTag("interactable") && Input.GetKeyDown(KeyCode.RightControl))
                {
                    interactable.Interact(playerID);

                }
            }
        }
        
        
    }

    private void OnTriggerEnter(Collider other)
    {
        interactable = other.GetComponent<IInteractable>();
        collider = other;
        playerInRange = true;

    }
    private void OnTriggerExit(Collider other)
    {

        playerInRange = false;

    }
}
