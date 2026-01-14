using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WoodInventory : MonoBehaviour
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

    LogMove lm = null;
   

    public LogMove oakLog;
    public LogMove birchLog;
    public LogMove pineLog;
    public LogMove oakPlank;
    public LogMove birchPlank;
    public LogMove pinePlank;
    public Canvas canvas;
   



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
    }

    public void AddWood(string treeType, int playerID)
    {
        if (treeType =="Oak")
        {
            getOak();
             lm = Instantiate(oakLog, canvas.transform);
        }
            
        if (treeType == "Pine")
        {
            getPine();
             lm = Instantiate(pineLog, canvas.transform);
        }
            
        if(treeType == "Birch")
        {
            getBirch();
             lm = Instantiate(birchLog, canvas.transform);
        }
        if (treeType == "OakPlank")
        {
            getOakPlank();
            lm = Instantiate(oakPlank, canvas.transform);
        }
        if (treeType == "PinePlank")
        {
            getPinePlank();
            lm = Instantiate(pinePlank, canvas.transform);
        }
        if (treeType == "BirchPlank")
        {
            getBirchPlank();
            lm = Instantiate(birchPlank, canvas.transform);
        }





        Vector2 playerPosition = new Vector2(0, 0);
        if(playerID == 1)
        {
            playerPosition.x = -Screen.width * 0.25f;
        }
        if(playerID == 2)
        {
            playerPosition.x = Screen.width * 0.25f;
        }
        if(lm != null)
        lm.StartMove(playerPosition);
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
    public void getOakPlank()
    {
        oakPlanks += 1;
       // oak -= 3;
    }
    public void getPinePlank()
    {
        pinePlanks += 1;
        pine -= 3;
    }
    public void getBirchPlank()
    {
        birchPlanks += 1;
        birch -= 3;
    }
}
