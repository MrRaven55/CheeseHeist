using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class LogGainAnimator : MonoBehaviour
{


    public GameObject oakLog;
    public GameObject birchLog;
    public GameObject pineLog;
    public GameObject oakPlank;
    public GameObject birchPlank;
    public GameObject pinePlank;
    public Canvas canvas;
    public void SpawnOakLog()
    {
        GameObject ui = Instantiate(oakLog, canvas.transform);
    }
    public void SpawnBirchLog()
    {
        GameObject ui2 = Instantiate(birchLog, canvas.transform);
    }
    public void SpawnPineLog()
    {
        GameObject ui3 = Instantiate(pineLog, canvas.transform);
    }
    public void SpawnOakPlank()
    {
        GameObject ui4 = Instantiate(oakPlank, canvas.transform);
    }
    public void SpawnBirchPlank()
    {
        GameObject ui5 = Instantiate(birchPlank, canvas.transform);
    }
    public void SpawnPinePlank()
    {
        GameObject ui6 = Instantiate(pinePlank, canvas.transform);
    }

   

    

    

}
