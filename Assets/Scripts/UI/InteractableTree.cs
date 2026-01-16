using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableTree : MonoBehaviour, IInteractable
{
    public string treeType;
    WoodInventory woodInventory;
    PlayerMovement players;
    public Minigame1 score;
    private IInteractable interactable;


    // Start is called before the first frame update
    void Start()
    {
        woodInventory = FindObjectOfType<WoodInventory>();
        players = GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
      
        if (score.score >= 40)
        {

            Interact(gameObject);
         

        }
    }
    public void Interact(GameObject player)
    {
        woodInventory.AddWood(treeType, player);
    }
}
