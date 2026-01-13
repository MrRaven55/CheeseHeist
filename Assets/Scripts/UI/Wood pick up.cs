using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Woodpickup : MonoBehaviour
{
    public int oak = 0;
    public int birch = 0;
    public int pine = 0;
    public int oakPlanks = 0;
    public int birchPlanks = 0;
    public int pinePlanks = 0;
    public int axeTier = 1;

    public TextMeshProUGUI oakUI;
    public TextMeshProUGUI birchUI;
    public TextMeshProUGUI pineUI;
    public TextMeshProUGUI oakPlanksUI;
    public TextMeshProUGUI birchPlanksUI;
    public TextMeshProUGUI pinePlanksUI;

    public LogGainAnimator spawnUI;
    
    

   


    // Start is called before the first frame update
    void Start()
    {
        
    }
   

    // Update is called once per frame
    void Update()
    {
        //sets the UI texts to correct wordings
        oakUI.text = "Oak: " + oak;
        birchUI.text = "Birch: " + birch;
        pineUI.text = "Pine: " + pine;
        oakPlanksUI.text = "Oak Planks: " + oakPlanks;
        birchPlanksUI.text = "Birch Planks: " + birchPlanks;
        pinePlanksUI.text = "Pine Planks: " + pinePlanks;
        //place holders
        if (Input.GetKeyDown(KeyCode.O))
        {
            getOak();
            spawnUI.SpawnOakLog();
            
        }
           
        if (Input.GetKeyDown(KeyCode.B))
        {
            getBirch();
            spawnUI.SpawnBirchLog();
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            getPine();
            spawnUI.SpawnPineLog();
        }
        if(Input.GetKeyDown(KeyCode.L) && oak >= 3)
        {
            oakPlanks += 1;
            oak -= 3;
            spawnUI.SpawnOakPlank();
        }
        if (Input.GetKeyDown(KeyCode.K) && birch >= 3)
        {
            birchPlanks += 1;
            birch -= 3;
            spawnUI.SpawnBirchPlank();
        }
        if (Input.GetKeyDown(KeyCode.J) && pine >= 3)
        {
            pinePlanks += 1;
            pine -= 3;
            spawnUI.SpawnPinePlank();
        }

    }
    //refrenses for easy access for mini games
    public void getOak()
    {
        oak += axeTier;
        
    }
    public void getBirch()
    {
        birch += axeTier;
    }
    public void getPine()
    {
        pine += axeTier;
    }

      





}

