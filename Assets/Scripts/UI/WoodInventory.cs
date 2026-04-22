using TMPro;
using UnityEngine;

/// <summary>
/// Central wood inventory. Counts are private but exposed via read-only properties.
/// Use AddWood(...) to change amounts and trigger UI animations.
/// </summary>
public class WoodInventory : MonoBehaviour
{
    /* ========== RESOURCE COUNTS (serialized but private) ========== */
    [Header("Raw Wood (counts)")]
    [SerializeField] private int oak;
    [SerializeField] private int birch;
    [SerializeField] private int pine;

    [Header("Planks (counts)")]
    [SerializeField] private int oakPlanks;
    [SerializeField] private int birchPlanks;
    [SerializeField] private int pinePlanks;
    [SerializeField] private int bridgePieces;

    [Header("Tools")]
    [SerializeField] private int axeTier = 1; // amount gained per chop = axeTier

    /* ===================== UI REFS ===================== */
    [Header("UI Text (optional)")]
    [SerializeField] private TextMeshProUGUI oakUI;
    [SerializeField] private TextMeshProUGUI birchUI;
    [SerializeField] private TextMeshProUGUI pineUI;
    [SerializeField] private TextMeshProUGUI oakPlanksUI;
    [SerializeField] private TextMeshProUGUI birchPlanksUI;
    [SerializeField] private TextMeshProUGUI pinePlanksUI;

    [Header("Log / Plank UI animations (optional prefabs)")]
    [SerializeField] private LogMove oakLog;
    [SerializeField] private LogMove birchLog;
    [SerializeField] private LogMove pineLog;
    [SerializeField] private LogMove oakPlank;
    [SerializeField] private LogMove birchPlank;
    [SerializeField] private LogMove pinePlank;

    [SerializeField] private Canvas canvas; // parent for animated logs/planks

    private LogMove activeLogMove;

    /* ========== PUBLIC READ-ONLY PROPERTIES ========== */
    public int Oak => oak;
    public int Birch => birch;
    public int Pine => pine;

    public int OakPlanks => oakPlanks;
    public int BirchPlanks => birchPlanks;
    public int PinePlanks => pinePlanks;
    public int Planks => oakPlanks + birchPlanks + pinePlanks;
    public int BridgePieces => bridgePieces;


    private void Update()
    {
        UpdateUI();
    }

    /// <summary>
    /// Centralized method to add wood or planks. Type strings are kept for compatibility.
    /// Valid types: "Oak", "Birch", "Pine", "OakPlank", "BirchPlank", "PinePlank"
    /// </summary>
    public void AddWood(string type, GameObject player)
    {
        activeLogMove = null;

        switch (type)
        {
            case "Oak":
                oak += axeTier;
                activeLogMove = InstantiateIfAssigned(oakLog);
                break;

            case "Birch":
                birch += axeTier;
                activeLogMove = InstantiateIfAssigned(birchLog);
                break;

            case "Pine":
                pine += axeTier;
                activeLogMove = InstantiateIfAssigned(pineLog);
                break;

            case "OakPlank":
                TryCraftPlankFromWoodType("Oak", 2);
                break;

            case "BirchPlank":
                TryCraftPlankFromWoodType("Birch", 2);
                break;

            case "PinePlank":
                TryCraftPlankFromWoodType("Pine", 2);
                break;

            case "Plank":
                TryCraftPlankFromAnyWood(2);
                break;

            default:
                Debug.LogWarning($"WoodInventory.AddWood: Unknown type '{type}'", this);
                break;
        }

        // If an animation prefab was assigned and a player passed in, animate it
        if (activeLogMove != null && player != null)
        {
            Vector2 startPos = GetPlayerUIPosition(player);
            activeLogMove.StartMove(startPos);
        }
    }

    public bool TryCraftPlankFromAnyWood(int logsRequiredPerPlank = 2)
    {
        activeLogMove = null;

        int totalRawWood = oak + birch + pine;
        if (totalRawWood < logsRequiredPerPlank) return false;

        int remaining = logsRequiredPerPlank;

        int takeOak = Mathf.Min(oak, remaining);
        oak -= takeOak;
        remaining -= takeOak;

        int takeBirch = Mathf.Min(birch, remaining);
        birch -= takeBirch;
        remaining -= takeBirch;

        int takePine = Mathf.Min(pine, remaining);
        pine -= takePine;

        // Keep existing UI references compatible by placing crafted pooled plank into oakPlanks.
        oakPlanks++;
        activeLogMove = InstantiateIfAssigned(oakPlank);

        return true;
    }

    public bool TryCraftPlankFromWoodType(string woodType, int logsRequiredPerPlank = 2)
    {
        activeLogMove = null;

        switch (woodType)
        {
            case "Oak":
                if (oak < logsRequiredPerPlank) return false;
                oak -= logsRequiredPerPlank;
                oakPlanks++;
                activeLogMove = InstantiateIfAssigned(oakPlank);
                return true;

            case "Birch":
                if (birch < logsRequiredPerPlank) return false;
                birch -= logsRequiredPerPlank;
                birchPlanks++;
                activeLogMove = InstantiateIfAssigned(birchPlank);
                return true;

            case "Pine":
                if (pine < logsRequiredPerPlank) return false;
                pine -= logsRequiredPerPlank;
                pinePlanks++;
                activeLogMove = InstantiateIfAssigned(pinePlank);
                return true;

            default:
                return false;
        }
    }

    public bool TryCraftBridgePiece(int planksRequired = 2)
    {
        if (!TryConsumePlanks(planksRequired)) return false;

        bridgePieces++;
        return true;
    }

    public bool TryConsumePlanks(int amount)
    {
        if (amount <= 0) return true;
        if (Planks < amount) return false;

        int remaining = amount;

        int takeOak = Mathf.Min(oakPlanks, remaining);
        oakPlanks -= takeOak;
        remaining -= takeOak;

        int takeBirch = Mathf.Min(birchPlanks, remaining);
        birchPlanks -= takeBirch;
        remaining -= takeBirch;

        int takePine = Mathf.Min(pinePlanks, remaining);
        pinePlanks -= takePine;

        return true;
    }

    // Instantiate helper that ensures canvas and prefab exist
    private LogMove InstantiateIfAssigned(LogMove prefab)
    {
        if (prefab == null || canvas == null) return null;
        return Instantiate(prefab, canvas.transform);
    }

    // Convert a player's PlayerMovement.playerID into a UI start position
    private Vector2 GetPlayerUIPosition(GameObject player)
    {
        Vector2 pos = Vector2.zero;
        PlayerMovement pm = player.GetComponent<PlayerMovement>();
        if (pm == null) return pos;

        if (pm.PlayerID == 1)
            pos.x = -Screen.width * 0.25f;
        else if (pm.PlayerID == 2)
            pos.x = Screen.width * 0.25f;

        pos.y = 0f;
        return pos;
    }

    // Update UI text once per frame (cheap, but fine for small projects).
    // You could optimize to update only when values change.
    private void UpdateUI()
    {
        if (oakUI) oakUI.text = $"Oak: {oak}";
        if (birchUI) birchUI.text = $"Birch: {birch}";
        if (pineUI) pineUI.text = $"Pine: {pine}";

        if (oakPlanksUI) oakPlanksUI.text = $"Oak Planks: {oakPlanks}";
        if (birchPlanksUI) birchPlanksUI.text = $"Birch Planks: {birchPlanks}";
        if (pinePlanksUI) pinePlanksUI.text = $"Pine Planks: {pinePlanks} | Total: {Planks} | Bridge: {bridgePieces}";
    }
}
