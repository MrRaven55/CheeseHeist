using DG.Tweening;
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

    public int AxeTier
    {
        get => axeTier;
        set => axeTier = Mathf.Max(1, value);
    }

    private void Update()
    {
        UpdateUI();
    }
    public enum WoodType
    {
        Oak,
        Birch,
        Pine,
        OakPlank,
        BirchPlank,
        PinePlank
    }

    public void ChangeAmount(WoodType type, int amount)
    {
        switch (type)
        {
            case WoodType.Oak:
                oak = Mathf.Max(0, oak + amount);
                break;
            case WoodType.Birch:
                birch = Mathf.Max(0, birch + amount);
                break;
            case WoodType.Pine:
                pine = Mathf.Max(0, pine + amount);
                break;
            case WoodType.OakPlank:
                oakPlanks = Mathf.Max(0, oakPlanks + amount);
                break;
            case WoodType.BirchPlank:
                birchPlanks = Mathf.Max(0, birchPlanks + amount);
                break;
            case WoodType.PinePlank:
                pinePlanks = Mathf.Max(0, pinePlanks + amount);
                break;
        }
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
                if (oak >= 3)
                {
                    oak -= 3;
                    oakPlanks++;
                    activeLogMove = InstantiateIfAssigned(oakPlank);
                }
                break;

            case "BirchPlank":
                if (birch >= 3)
                {
                    birch -= 3;
                    birchPlanks++;
                    activeLogMove = InstantiateIfAssigned(birchPlank);
                }
                break;

            case "PinePlank":
                if (pine >= 3)
                {
                    pine -= 3;
                    pinePlanks++;
                    activeLogMove = InstantiateIfAssigned(pinePlank);
                }
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
        if (pinePlanksUI) pinePlanksUI.text = $"Pine Planks: {pinePlanks}";
    }
}
