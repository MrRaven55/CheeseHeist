using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestWorkerBoard : MonoBehaviour, IInteractable
{
    public KeyCode upButton;


    public string woodType; //pine, birch or oak
    public PassiveWoodGain passiveWoodGain;

    private bool uiOpen = false;

    public void Interact(int playerID)
    {
        //open UI
        uiOpen = true;
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
            
        }
    }
}
