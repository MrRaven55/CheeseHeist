using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableTest : MonoBehaviour, IInteractable
{
    
   public void Interact(GameObject player) 
    {
        Debug.Log("PlayerID: " + player);
    }

}
