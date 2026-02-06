using TMPro;
using UnityEngine;

public class WoodInventory : MonoBehaviour
{
    /* ===================== RESOURCES ===================== */
    [Header("Raw Wood")]
    public int oak;
    public int birch;
    public int pine;

    [Header("Planks")]
    public int oakPlanks;
    public int birchPlanks;
    public int pinePlanks;

    [Header("Tools")]
    public int axeTier = 1;

    /* ===================== UI ===================== */
    [Header("UI Text")]
    public TextMeshProUGUI oakUI;
    public TextMeshProUGUI birchUI;
    public TextMeshProUGUI pineUI;
    public TextMeshProUGUI oakPlanksUI;
    public TextMeshProUGUI birchPlanksUI;
    public TextMeshProUGUI pinePlanksUI;

    /* ===================== UI ANIMATIONS ===================== */
    [Header("Log / Plank Animations")]
    public LogMove oakLog;
    public LogMove birchLog;
    public LogMove pineLog;

    public LogMove oakPlank;
    public LogMove birchPlank;
    public LogMove pinePlank;

    public Canvas canvas;
    private LogMove activeLogMove;

    void Update()
    {
        UpdateUI();
    }

    /* ===================== PUBLIC API ===================== */
    public void AddWood(string type, GameObject player)
    {
        activeLogMove = null;

        switch (type)
        {
            case "Oak":
                oak += axeTier;
                activeLogMove = Instantiate(oakLog, canvas.transform);
                break;

            case "Birch":
                birch += axeTier;
                activeLogMove = Instantiate(birchLog, canvas.transform);
                break;

            case "Pine":
                pine += axeTier;
                activeLogMove = Instantiate(pineLog, canvas.transform);
                break;

            case "OakPlank":
                if (oak >= 3)
                {
                    oak -= 3;
                    oakPlanks++;
                    activeLogMove = Instantiate(oakPlank, canvas.transform);
                }
                break;

            case "BirchPlank":
                if (birch >= 3)
                {
                    birch -= 3;
                    birchPlanks++;
                    activeLogMove = Instantiate(birchPlank, canvas.transform);
                }
                break;

            case "PinePlank":
                if (pine >= 3)
                {
                    pine -= 3;
                    pinePlanks++;
                    activeLogMove = Instantiate(pinePlank, canvas.transform);
                }
                break;
        }

        if (activeLogMove != null && player != null)
        {
            Vector2 startPos = GetPlayerUIPosition(player);
            activeLogMove.StartMove(startPos);
        }
    }

    /* ===================== HELPERS ===================== */
    private Vector2 GetPlayerUIPosition(GameObject player)
    {
        Vector2 pos = Vector2.zero;
        PlayerMovement pm = player.GetComponent<PlayerMovement>();

        if (pm == null) return pos;

        if (pm.playerID == 1)
            pos.x = -Screen.width * 0.25f;
        else if (pm.playerID == 2)
            pos.x = Screen.width * 0.25f;

        return pos;
    }

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
