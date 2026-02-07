using UnityEngine;

/// <summary>
/// Handles player proximity-based interaction with IInteractable objects and
/// triggers Minigame1 (sets up the minigame parameters before enabling it).
/// </summary>
public class PlayerInteraction : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private bool playerInRange;
    private IInteractable interactable;

    [Header("References")]
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private GameObject minigame1;  // minigame prefab or panel (set in inspector)
    [SerializeField] private GameObject minigame2;
    [SerializeField] private WoodInventory woodInventory;               // optional, auto-found if null

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
            if (interactable == null || minigame1 == null) return;

            // Pass references into the minigame before starting it
            Minigame1 mg = minigame1.GetComponent<Minigame1>();
            if (mg != null)
            {
                mg.PlayerRef = gameObject;
                mg.WoodInventory = woodInventory;

                // If interacting with an InteractableTree, pass the tree type for rewards
                if (interactable is InteractableTree tree)
                {
                    mg.NormalHitWoodType = tree.TreeTypeName;
                    mg.PerfectHitWoodType = tree.TreeTypeName + "Plank";
                }
            }

            minigame1.SetActive(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
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
}
