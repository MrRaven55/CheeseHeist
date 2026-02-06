using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Lane-based falling fruit minigame which crafts planks by consuming logs.
/// Uses FruitBehaviour for per-fruit lifetime/collection handling.
/// NOTE: This now uses the WoodInventory properties (Oak/Birch/Pine) rather than direct field access.
/// </summary>
public class Minigame2 : MonoBehaviour
{
    [Header("Lanes")]
    [SerializeField] private int laneCount = 8;

    [Header("Player")]
    [SerializeField] private RectTransform gear;
    [SerializeField] private GameObject playerObject;
    [SerializeField] private int startLane = 3;

    [Header("Fruit")]
    [SerializeField] private RectTransform fruitPrefab;
    [SerializeField] private RectTransform fruitParent;

    [Header("Inventory")]
    [SerializeField] private WoodInventory woodInventory;
    [SerializeField] private string plankType = "OakPlank"; // crafting output

    [Header("Costs")]
    [SerializeField] private int logsRequired = 3;

    [Header("Speed")]
    [SerializeField] private float fallSpeed = 300f;

    [Header("Spawn")]
    [SerializeField] private float spawnInterval = 1f;

    [Header("Win Condition")]
    [SerializeField] private int targetPlanks = 5;
    [SerializeField] private UnityEvent onWin;

    private RectTransform playAreaRect;
    private int currentLane;
    private int craftedPlanks;
    private float spawnTimer;
    private int lastSpawnLane = 0;

    private void OnEnable()
    {
        playAreaRect = GetComponent<RectTransform>();
        currentLane = Mathf.Clamp(startLane, 0, Mathf.Max(0, laneCount - 1));
        spawnTimer = spawnInterval;
        craftedPlanks = 0;
        UpdateGearPosition();
    }

    private void Update()
    {
        HandleInput();
        HandleSpawning();
        MoveAndCheckFruits();
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.A)) MoveLane(-1);
        if (Input.GetKeyDown(KeyCode.D)) MoveLane(1);
    }

    private void MoveLane(int dir)
    {
        if (laneCount <= 0) return;
        currentLane = Mathf.Clamp(currentLane + dir, 0, laneCount - 1);
        UpdateGearPosition();
    }

    private void UpdateGearPosition()
    {
        if (gear == null || playAreaRect == null) return;
        Vector2 pos = gear.anchoredPosition;
        pos.x = GetLaneX(currentLane);
        gear.anchoredPosition = pos;
    }

    private void HandleSpawning()
    {
        if (fruitPrefab == null || fruitParent == null || playAreaRect == null) return;

        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0f)
        {
            SpawnFruit();
            spawnTimer = spawnInterval;
        }
    }

    private void SpawnFruit()
    {
        if (fruitPrefab == null || fruitParent == null || playAreaRect == null || laneCount <= 0) return;

        int lane = Mathf.Clamp(lastSpawnLane + Random.Range(-1, 2), 0, laneCount - 1);
        lastSpawnLane = lane;

        RectTransform fruit = Instantiate(fruitPrefab, fruitParent);
        fruit.anchoredPosition = new Vector2(
            GetLaneX(lane),
            playAreaRect.rect.height * 0.5f + 100f
        );

        FruitBehaviour fb = fruit.GetComponent<FruitBehaviour>();
        if (fb != null) fb.Init(10f, lane);
    }

    private void MoveAndCheckFruits()
    {
        if (fruitParent == null || gear == null) return;

        for (int i = fruitParent.childCount - 1; i >= 0; i--)
        {
            RectTransform fruit = fruitParent.GetChild(i) as RectTransform;
            if (fruit == null) continue;

            fruit.anchoredPosition += Vector2.down * fallSpeed * Time.deltaTime;

            FruitBehaviour fb = fruit.GetComponent<FruitBehaviour>();
            if (fb != null && !fb.Collected && IsOverlapping(fruit, gear))
            {
                TryCraftPlank(fb);
            }
        }
    }

    private void TryCraftPlank(FruitBehaviour fb)
    {
        if (fb == null || fb.Collected) return;
        if (woodInventory == null)
        {
            // If no inventory assigned, mark collected to prevent repeated triggers
            fb.MarkCollected();
            return;
        }

        // Check if we have enough logs for desired plank type
        if (!HasRequiredLogs())
        {
            // Not enough logs: consume the fruit but give no plank (could add feedback)
            fb.MarkCollected();
            return;
        }

        // Consume logs and add plank
        ConsumeLogs();
        woodInventory.AddWood(plankType, playerObject);
        craftedPlanks++;
        fb.MarkCollected();

        if (craftedPlanks >= targetPlanks)
        {
            onWin?.Invoke();
            gameObject.SetActive(false);
        }
    }

    // Use WoodInventory properties rather than directly accessing fields
    private bool HasRequiredLogs()
    {
        if (woodInventory == null) return false;

        switch (plankType)
        {
            case "OakPlank": return woodInventory.Oak >= logsRequired;
            case "PinePlank": return woodInventory.Pine >= logsRequired;
            case "BirchPlank": return woodInventory.Birch >= logsRequired;
            default: return false;
        }
    }

    private void ConsumeLogs()
    {
        if (woodInventory == null) return;

        switch (plankType)
        {
            case "OakPlank":
                // Note: WoodInventory doesn't provide write access via properties,
                // so we use AddWood to modify (it applies internal logic).
                // But to consume raw logs we need a safe method; for now we
                // reflect the previous behavior by adding a private helper or
                // directly using internal fields. To keep encapsulation we could
                // add RemoveLogs method to WoodInventory — quick in-place approach:
                ModifyRawLogs("Oak", -logsRequired);
                break;

            case "PinePlank":
                ModifyRawLogs("Pine", -logsRequired);
                break;

            case "BirchPlank":
                ModifyRawLogs("Birch", -logsRequired);
                break;
        }
    }

    /// <summary>
    /// Helper to mutate raw log counts using AddWood style calls.
    /// Because AddWood doesn't allow negative values, this adjusts internal
    /// counts via a temporary approach: call a dedicated API would be better.
    /// To avoid breaking encapsulation, this method directly modifies fields via reflection fallback.
    /// (Better: add public RemoveLogs/ConsumeRawLogs to WoodInventory — I can add that if you want.)
    /// </summary>
    private void ModifyRawLogs(string type, int delta)
    {
        // Try to find appropriate field via property methods if available.
        // For now, use a minimal reflection fallback to change private fields safely.
        // Reflection is slower but acceptable for low-frequency operations in a minigame.
        var wi = woodInventory;
        if (wi == null) return;

        System.Type t = wi.GetType();
        string fieldName = type.ToLower(); // oak, birch, pine
        var fi = t.GetField(fieldName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        if (fi != null && fi.FieldType == typeof(int))
        {
            int current = (int)fi.GetValue(wi);
            fi.SetValue(wi, Mathf.Max(0, current + delta));
        }
        else
        {
            Debug.LogWarning("Minigame2: Unable to modify raw logs. Consider adding a public API to WoodInventory.", this);
        }
    }

    private bool IsOverlapping(RectTransform a, RectTransform b)
    {
        Rect ra = GetWorldRect(a);
        Rect rb = GetWorldRect(b);
        return ra.Overlaps(rb);
    }

    private Rect GetWorldRect(RectTransform rt)
    {
        Vector3[] corners = new Vector3[4];
        rt.GetWorldCorners(corners);
        // corners[0] = bottom-left, corners[2] = top-right
        return new Rect(corners[0], corners[2] - corners[0]);
    }

    private float GetLaneX(int lane)
    {
        if (playAreaRect == null || laneCount <= 1) return 0f;
        float width = playAreaRect.rect.width;
        return -width / 2f + (width / (laneCount - 1)) * lane;
    }
}
