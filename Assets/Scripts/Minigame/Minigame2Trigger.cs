using UnityEngine;

/// <summary>
/// Interactable trigger that starts Minigame2 through PlayerInteraction.
/// </summary>
public class Minigame2Trigger : MonoBehaviour, IInteractable
{
    [SerializeField] private WoodInventory woodInventory;

    private void Awake()
    {
        if (woodInventory == null)
            woodInventory = FindObjectOfType<WoodInventory>();
    }

    public void Interact(GameObject player)
    {
        if (player == null) return;

        PlayerInteraction playerInteraction = player.GetComponent<PlayerInteraction>();
        if (playerInteraction == null) return;

        GameObject minigame2Object = playerInteraction.Minigame2Object;
        if (minigame2Object == null)
        {
            Debug.LogWarning("Minigame2Trigger: Minigame2 is not assigned on PlayerInteraction.", this);
            return;
        }

        Minigame2 minigame2 = minigame2Object.GetComponent<Minigame2>();
        PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();

        if (minigame2 != null && playerMovement != null)
        {
            minigame2.ConfigureSession(player, playerMovement.PlayerID, woodInventory);
            PlayerMovement.SetMovementEnabledForPlayer(playerMovement.PlayerID, false);
        }

        minigame2Object.SetActive(true);
    }
}
