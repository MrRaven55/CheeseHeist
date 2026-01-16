using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class PlayerInteraction : MonoBehaviour
{

    PlayerMovement pM;
    public  WoodInventory wI;
    public KeyCode interact;
    public string treeType;

    int playerID;
    private bool playerInRange;
    private IInteractable interactable;
    public GameObject minigame1;

    private void Awake()
    {
        pM = gameObject.GetComponent<PlayerMovement>();
        


        playerID = pM.playerID;
    }

    private void Update()
    {
        if (playerInRange)
        {
           
                if (Input.GetKeyDown(interact))
                {
                // interactable.Interact(gameObject);
                     minigame1.SetActive(true);
              

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
