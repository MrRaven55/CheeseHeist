using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    private PlayerMovement pM;
    private bool playerInRange;
    private IInteractable interactable;

    [Header("References")]
    public KeyCode interact = KeyCode.E;
    public GameObject minigame1;
    public WoodInventory wI;

    private void Awake()
    {
        pM = GetComponent<PlayerMovement>();
        if (wI == null)
            wI = FindObjectOfType<WoodInventory>();
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(interact))
        {
            if (interactable != null)
            {
                // Pass player reference to minigame
                Minigame1 mg = minigame1.GetComponent<Minigame1>();
                if (mg != null)
                {
                    mg.PlayerRef = gameObject;       // current player
                    mg.WoodInventory = wI;          // assign the inventory
                    // assign tree type to minigame rewards
                    if (interactable is InteractableTree tree)
                    {
                        mg.NormalHitWoodType = tree.treeType;        // normal hit
                        mg.PerfectHitWoodType = tree.treeType + "Plank"; // perfect hit
                    }
                }

                minigame1.SetActive(true); // start minigame
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("interactable"))
        {
            interactable = other.GetComponent<IInteractable>();
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("interactable"))
        {
            interactable = null;
            playerInRange = false;
        }
    }
}
