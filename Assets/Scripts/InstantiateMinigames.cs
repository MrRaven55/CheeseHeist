using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateMinigames : MonoBehaviour
{

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

                // Get component from parent gameprefab identifier
            }
        }
    }

    
}
