using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{

   public void Interact(GameObject player);
    //player.GetComponent<PlayerMovement>().isInMenu = true;
    //make a button to leave a menu, [playermovementvariable].downButton is the standard for this
}
