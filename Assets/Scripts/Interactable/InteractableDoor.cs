using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableDoor : MonoBehaviour, IInteractable
{
    public WoodInventory wI;
    [Header("Raw Wood (costs)")]
    [SerializeField] private int oakCost;
    [SerializeField] private int birchCost;
    [SerializeField] private int pineCost;

    [Header("Planks (costs)")]
    [SerializeField] private int oakPlanksCost;
    [SerializeField] private int birchPlanksCost;
    [SerializeField] private int pinePlanksCost;

   

    public void Interact(GameObject player)
    {
        wI = FindObjectOfType<WoodInventory>();
        
        if(wI.Oak >= oakCost && wI.Birch >= birchCost && wI.Pine >= pineCost &&
          wI.OakPlanks >= oakPlanksCost && wI.BirchPlanks >= birchPlanksCost &&
          wI.PinePlanks >= pinePlanksCost)
          {
            wI.ChangeAmount(WoodInventory.WoodType.Oak, -oakCost);
            wI.ChangeAmount(WoodInventory.WoodType.Birch, -birchCost);
            wI.ChangeAmount(WoodInventory.WoodType.Pine, -pineCost);

            wI.ChangeAmount(WoodInventory.WoodType.OakPlank, -oakPlanksCost);
            wI.ChangeAmount(WoodInventory.WoodType.OakPlank, -birchPlanksCost);
            wI.ChangeAmount(WoodInventory.WoodType.OakPlank, -pinePlanksCost);


            Destroy(gameObject);
          }
    }

}
