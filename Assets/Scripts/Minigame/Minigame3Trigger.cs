using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Interactable trigger that starts Minigame3 through PlayerInteraction.
/// </summary>
public class Minigame3Trigger : MonoBehaviour, IInteractable
{
    [SerializeField] private Minigame3 minigame3;
    [SerializeField] private UnityEvent onNeedBothPlayers;

    private readonly HashSet<GameObject> playersInZone = new HashSet<GameObject>();
    private int playerLayer = -1;

    private void Awake()
    {
        playerLayer = LayerMask.NameToLayer("Player");
        if (playerLayer < 0)
        {
            Debug.LogWarning("Minigame3Trigger: Layer 'Player' was not found.", this);
        }
    }

    public void Interact(GameObject player)
    {
        if (minigame3 == null)
        {
            Debug.LogWarning("Minigame3Trigger: Minigame3 reference is missing.", this);
            return;
        }

        if (minigame3.IsActive) return;

        if (!HasEnoughPlayersInZone())
        {
            onNeedBothPlayers?.Invoke();
            Debug.Log("Minigame3Trigger: Two player-layer objects must be inside the trigger to start Minigame3.", this);
            return;
        }

        minigame3.StartMinigame();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsPlayerLayerObject(other.gameObject)) return;

        GameObject playerObject = ResolvePlayerObject(other);
        playersInZone.Add(playerObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsPlayerLayerObject(other.gameObject)) return;

        GameObject playerObject = ResolvePlayerObject(other);
        playersInZone.Remove(playerObject);
    }

    private bool HasEnoughPlayersInZone()
    {
        return playersInZone.Count >= 2;
    }

    private bool IsPlayerLayerObject(GameObject obj)
    {
        return playerLayer >= 0 && obj.layer == playerLayer;
    }

    private GameObject ResolvePlayerObject(Collider other)
    {
        PlayerMovement pm = other.GetComponentInParent<PlayerMovement>();
        if (pm != null) return pm.gameObject;

        if (other.attachedRigidbody != null) return other.attachedRigidbody.gameObject;

        return other.transform.root.gameObject;
    }
}
