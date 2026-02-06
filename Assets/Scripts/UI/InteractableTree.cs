using UnityEngine;

/// <summary>
/// Interactable tree that a player can chop. Uses an enum for type-safety
/// but exposes a string name for compatibility with existing systems.
/// </summary>
public enum TreeType { Oak, Birch, Pine }

public class InteractableTree : MonoBehaviour, IInteractable
{
    [Header("Tree Settings")]
    [Tooltip("Type of tree: Oak, Birch, Pine")]
    [SerializeField] private TreeType treeType = TreeType.Pine;

    // cached reference to inventory (found at Start)
    private WoodInventory woodInventory;

    /// <summary>
    /// String version used by existing code (Minigame1 expects a string).
    /// </summary>
    public string TreeTypeName => treeType.ToString();

    private void Start()
    {
        // Attempt to locate the WoodInventory once
        woodInventory = FindObjectOfType<WoodInventory>();
        if (woodInventory == null)
            Debug.LogWarning("InteractableTree: WoodInventory not found in scene!", this);
    }

    /// <summary>
    /// Called by the interaction system. Uses the cached WoodInventory to add resources.
    /// </summary>
    public void Interact(GameObject player)
    {
        if (woodInventory == null || player == null) return;

        Debug.Log($"Player {player.name} chopped {TreeTypeName} tree!", this);
        woodInventory.AddWood(TreeTypeName, player);
    }
}
