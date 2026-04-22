using UnityEngine;
using System;
/// <summary>
/// Handles player proximity-based interaction with IInteractable objects and
/// triggers Minigame1 (sets up the minigame parameters before enabling it).
/// </summary>
public class PlayerInteraction : MonoBehaviour
{
    public static event Action<int> OnInteract;
    private PlayerMovement playerMovement;
    private bool playerInRange;
    private IInteractable interactable;

    [Header("References")]
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private GameObject minigame1;  // minigame prefab or panel (set in inspector)
    [SerializeField] private GameObject minigame2;
    [SerializeField] private WoodInventory woodInventory;               // optional, auto-found if null
    [SerializeField] private int currentPlayerID;

    public GameObject Minigame2Object => minigame2;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();

        if (woodInventory == null)
            woodInventory = FindObjectOfType<WoodInventory>();
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(interactKey))
        {
            if (interactable == null) return;

            // Minigame triggers use the same IInteractable flow as trees.
            if (interactable is Minigame2Trigger || interactable is Minigame3Trigger)
            {
                interactable.Interact(gameObject);
                return;
            }

            if (minigame1 == null) return;

            // Pass references into the minigame before starting it
            Minigame1 mg = minigame1.GetComponent<Minigame1>();
            if (mg != null)
            {
                if(currentPlayerID == playerMovement.PlayerID) 
                { 
                OnInteract?.Invoke(currentPlayerID);
                Debug.Log("Current ID interacted is" + currentPlayerID);
                }
                mg.PlayerId = playerMovement.PlayerID;
                mg.PlayerRef = gameObject;
                mg.WoodInventory = woodInventory;

                // If interacting with an InteractableTree, pass the tree type for rewards
                if (interactable is InteractableTree tree)
                {
                    mg.NormalHitWoodType = tree.TreeTypeName;
                    mg.PerfectHitWoodType = tree.TreeTypeName + "Plank";
                }
            }

            if (playerMovement != null)
            {
                PlayerMovement.SetMovementEnabledForPlayer(playerMovement.PlayerID, false);
            }

            minigame1.SetActive(true);
        }
        MinigameController();
        DoorUnlock();
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            PlayerMovement ID = other.GetComponent<PlayerMovement>();
            if( ID != null)
            {
                currentPlayerID = ID.PlayerID;
            }
        }

        if (other.CompareTag("interactable"))
        {
            IInteractable i = other.GetComponent<IInteractable>();
            if (i != null)
            {
                interactable = i;
                playerInRange = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("interactable"))
        {
            IInteractable i = other.GetComponent<IInteractable>();
            if (i == interactable)
            {
                interactable = null;
                playerInRange = false;
            }
        }
    }

    private void MinigameController()
    {
        if (playerInRange && Input.GetKeyDown(interactKey))
        {
            if (interactable == null || minigame1 == null) return;

            // Pass references into the minigame before starting it
            Minigame1 mg = minigame1.GetComponent<Minigame1>();
            if (mg != null)
            {
                /* if (currentPlayerID == playerMovement.PlayerID)
                {
                    OnInteract?.Invoke(currentPlayerID);
                    Debug.Log("Current ID interacted is" + currentPlayerID);
                } */
                mg.PlayerRef = gameObject;
                mg.WoodInventory = woodInventory;

                // If interacting with an InteractableTree, pass the tree type for rewards
                if (interactable is InteractableTree tree)
                {
                    mg.NormalHitWoodType = tree.TreeTypeName;
                    mg.PerfectHitWoodType = tree.TreeTypeName + "Plank";
                    minigame1.SetActive(true);
                }
            }

            
        }
    }

    private void DoorUnlock()
    {
        if (playerInRange && Input.GetKeyDown(interactKey))
        {
            Debug.Log("Interacted");
            if (interactable == null) return;

            if(interactable is InteractableDoor door)
            {
                Debug.Log("Interacted with door");

                door.Interact(gameObject);
            }

        }
    }
}
