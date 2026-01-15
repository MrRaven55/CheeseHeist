using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigameIdentifier : MonoBehaviour
{
   
    [SerializeField] private GameObject minigamePrefab1;
    [SerializeField] private GameObject minigamePrefab2;
    [SerializeField] private GameObject minigamePrefab3;

    void Start()
    {
       if(!minigamePrefab1 == null)
       {
           return;
       }
       else
       {
            GameObject minigamePrefab1 = GameObject.FindWithTag("Minigame1");
       }
       if (!minigamePrefab2 == null)
       {
            return;
       }
       else
       {
            GameObject minigamePrefab3 = GameObject.FindWithTag("Minigame2");
       }
       if (!minigamePrefab3 == null)
       {
            return;
       }
       else
       {
            GameObject minigamePrefab3 = GameObject.FindWithTag("Minigame3");
       }

    }


    void Update()
    {
       
    }


}
