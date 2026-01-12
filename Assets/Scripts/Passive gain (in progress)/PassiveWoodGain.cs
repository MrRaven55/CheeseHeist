using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class PassiveWoodGain : MonoBehaviour
{
    //price of upgrades should be exponential, and cost new wood types.

    //Pine = 1 * unitamount
    //Birch = 2/3 * unitamount
    //Oak = 1/3 * unitamount
        //maybe just make 3 different methods for each wood type



    // Start is called before the first frame update

    public int unit = 0; //standard amount gained from passive gain per time
    public float pRep = 10;
    public float bRep = 15;
    public float oRep = 20;  //time in seconds between each wood gain

    [SerializeField]
    private int workerAmount; //only incremented in "HireWorker". This is basically upgrade amount.
    public int PWorkers{get { return pWorkers; }private set { pWorkers = value; }}
    private int pWorkers;
    public int BWorkers{get { return bWorkers; }private set { bWorkers = value; }}
    private int bWorkers;
    public int OWorkers{get { return oWorkers; }private set { oWorkers = value; }}
    private int oWorkers;
    public int LeftoverWorkers { get { return leftoverWorkers; } private set { leftoverWorkers = value; }}
    private int leftoverWorkers;

    List<int> wCost = new List<int>() { 10, 0, 0, 0, 0, 0}; //p.log, b.log, o.log, p.plank, b.plank, o.plank
    public List<int> resourceBank = new List<int>() { 0,0,0,0,0,0};
    void Start()
    {
        InvokeRepeating("AmassPine", pRep, pRep);
        InvokeRepeating("AmassBirch", bRep, bRep);
        InvokeRepeating("AmassOak", oRep, oRep);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.H))
        {
            HireWorker();
        }
    }
    private void AmassPine()
    {
        int woodGained = pWorkers * unit;
        Debug.Log($"gained {woodGained} wood");
        //Sends signal to inventory to add more wood of (type)
    }
    private void AmassBirch()
    {
        int woodGained = bWorkers * unit;
        Debug.Log($"gained {woodGained} wood");
        //Sends signal to inventory to add more wood of (type)
    }
    private void AmassOak()
    {
        int woodGained = oWorkers * unit;
        Debug.Log($"gained {woodGained} wood");
        //Sends signal to inventory to add more wood of (type)
    }

    private void HireWorker()
    {
        bool canUpgrade = true;
        for(int i = 0; i > 6; i++)
        {
            if (resourceBank[i] !>= wCost[i])
            {
                Debug.Log($"you dont have enough resources in position {i}");
                canUpgrade = false;
            }
        }
        if (canUpgrade)
        {
            for (int i = 0; i > 6; i++)
            {
                resourceBank[i] -= wCost[i];
            }
            workerAmount += 1;
            Debug.Log($"current worker amount {workerAmount};");
            switch (workerAmount) //set cost to new cost
            {
                case 1:
                    wCost = new List<int>() { 20, 0, 0, 0, 0, 0 };
                    break;
                case 2:
                    wCost = new List<int>() { 30, 0, 0, 0, 0, 0 };
                    break;
                case 3:
                    wCost = new List<int>() { 30, 0, 0, 0, 0, 0 };
                    break;
                case 4:
                    wCost = new List<int>() { 40, 0, 0, 0, 0, 0 };
                    break;
                case 5:
                    wCost = new List<int>() { 50, 0, 0, 0, 0, 0 };
                    break;
                case 6:
                    wCost = new List<int>() { 60, 0, 0, 0, 0, 0 };
                    break;
                case 7:
                    wCost = new List<int>() { 70, 0, 0, 0, 0, 0 };
                    break;
                case 8:
                    wCost = new List<int>() { 80, 0, 0, 0, 0, 0 };
                    break;
                case 9:
                    wCost = new List<int>() { 90, 0, 0, 0, 0, 0 };
                    break;
                case 10:
                    wCost = new List<int>() { 100, 0, 0, 0, 0, 0 };
                    break;
                case 11:
                    wCost = new List<int>() { 110, 0, 0, 0, 0, 0 };
                    break;
                case 12:
                    wCost = new List<int>() { 120, 0, 0, 0, 0, 0 };
                    break;
                case 13:
                    wCost = new List<int>() { 120, 0, 0, 0, 0, 0 };
                    break;
                case 14:
                    wCost = new List<int>() { 20, 0, 0, 0, 0, 0 };
                    break;
                case 15:
                    wCost = new List<int>() { 20, 0, 0, 0, 0, 0 };
                    break;
                case 16:
                    wCost = new List<int>() { 20, 0, 0, 0, 0, 0 };
                    break;
                case 17:
                    wCost = new List<int>() { 20, 0, 0, 0, 0, 0 };
                    break;
                case 18:
                    wCost = new List<int>() { 20, 0, 0, 0, 0, 0 };
                    break;
                case 19:
                    wCost = new List<int>() { 20, 0, 0, 0, 0, 0 };
                    break;
                case 20:
                    wCost = new List<int>() { 20, 0, 0, 0, 0, 0 };
                    break;
            }
        }
    }
}
