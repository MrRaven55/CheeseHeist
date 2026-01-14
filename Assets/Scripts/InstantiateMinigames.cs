using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateMinigames : MonoBehaviour
{
    public static event Action<float>  startMinigame;
    void Start()
    {
        
    }


    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (other.CompareTag("Player"))
            {
                // for Oscar: Get player ID and change camera instantied minigame accordingly 
                // Call action event to start minigame(float is cam type)
                // Get component from parent gameprefab identifier
            }
        }
    }

    
}
