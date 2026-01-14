using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableTest : MonoBehaviour, IInteractable
{
    
   public void Interact(int playerID) 
    {
        Debug.Log("PlayerID: " + playerID);
    }

}
