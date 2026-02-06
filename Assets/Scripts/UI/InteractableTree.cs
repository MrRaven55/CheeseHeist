using UnityEngine;

public class InteractableTree : MonoBehaviour, IInteractable
{
    [Header("Tree Settings")]
    [Tooltip("Type of tree: Oak, Birch, Pine")]
    public string treeType = "Pine";

    private WoodInventory woodInventory;

    void Start()
    {
        woodInventory = FindObjectOfType<WoodInventory>();
        if (woodInventory == null)
            Debug.LogWarning("WoodInventory not found in scene!");
    }

    // Call this method when player interacts with the tree
    public void Interact(GameObject player)
    {
        if (woodInventory == null || player == null) return;

        Debug.Log($"Player {player.name} chopped {treeType} tree!");
        woodInventory.AddWood(treeType, player);
    }
}
