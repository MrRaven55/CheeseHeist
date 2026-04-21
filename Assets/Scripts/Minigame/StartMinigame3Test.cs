using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMinigame3Test : MonoBehaviour
{
    [SerializeField] private Minigame3 minigame3;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            minigame3.StartMinigame();
        }
    }
}
