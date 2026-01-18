using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableTree : MonoBehaviour, IInteractable
{
    public string treeType;
    WoodInventory woodInventory;
    public void Interact(int playerID)
    {
        woodInventory.AddWood(treeType, playerID);
    }

    public void Interact(GameObject player)
    {
        throw new System.NotImplementedException();
    }

    // Start is called before the first frame update
    void Start()
    {
        woodInventory = FindObjectOfType<WoodInventory>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Interact(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Interact(2);
        }
    }
}
