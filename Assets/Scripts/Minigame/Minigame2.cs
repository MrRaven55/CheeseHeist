using UnityEngine;
using UnityEngine.Events;

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
    public UnityEvent onWin;

    private RectTransform playAreaRect;
    private int currentLane;
    private int craftedPlanks;
    private float spawnTimer;
    private int lastSpawnLane;

    void OnEnable()
    {
        playAreaRect = GetComponent<RectTransform>();
        currentLane = startLane;
        spawnTimer = spawnInterval;
        craftedPlanks = 0;
        UpdateGearPosition();
    }

    void Update()
    {
        HandleInput();
        HandleSpawning();
        MoveAndCheckFruits();
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.A)) MoveLane(-1);
        if (Input.GetKeyDown(KeyCode.D)) MoveLane(1);
    }

    void MoveLane(int dir)
    {
        currentLane = Mathf.Clamp(currentLane + dir, 0, laneCount - 1);
        UpdateGearPosition();
    }

    void UpdateGearPosition()
    {
        Vector2 pos = gear.anchoredPosition;
        pos.x = GetLaneX(currentLane);
        gear.anchoredPosition = pos;
    }

    void HandleSpawning()
    {
        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0f)
        {
            SpawnFruit();
            spawnTimer = spawnInterval;
        }
    }

    void SpawnFruit()
    {
        int lane = Mathf.Clamp(lastSpawnLane + Random.Range(-1, 2), 0, laneCount - 1);
        lastSpawnLane = lane;

        RectTransform fruit = Instantiate(fruitPrefab, fruitParent);
        fruit.anchoredPosition = new Vector2(
            GetLaneX(lane),
            playAreaRect.rect.height * 0.5f + 100f
        );

        fruit.GetComponent<FruitBehaviour>().Init(10f, lane);
    }

    void MoveAndCheckFruits()
    {
        for (int i = fruitParent.childCount - 1; i >= 0; i--)
        {
            RectTransform fruit = fruitParent.GetChild(i) as RectTransform;
            fruit.anchoredPosition += Vector2.down * fallSpeed * Time.deltaTime;

            if (IsOverlapping(fruit, gear))
            {
                TryCraftPlank(fruit.GetComponent<FruitBehaviour>());
            }
        }
    }

    void TryCraftPlank(FruitBehaviour fb)
    {
        if (fb.collected) return;

        // ❌ Not enough logs → fail silently or add feedback
        if (!HasRequiredLogs())
        {
            fb.MarkCollected();
            return;
        }

        // ✅ Consume logs
        ConsumeLogs();

        // ✅ Add plank
        woodInventory.AddWood(plankType, playerObject);
        craftedPlanks++;

        fb.MarkCollected();

        if (craftedPlanks >= targetPlanks)
        {
            onWin?.Invoke();
            gameObject.SetActive(false);
        }
    }

    bool HasRequiredLogs()
    {
        if (plankType == "OakPlank") return woodInventory.oak >= logsRequired;
        if (plankType == "PinePlank") return woodInventory.pine >= logsRequired;
        if (plankType == "BirchPlank") return woodInventory.birch >= logsRequired;
        return false;
    }

    void ConsumeLogs()
    {
        if (plankType == "OakPlank") woodInventory.oak -= logsRequired;
        if (plankType == "PinePlank") woodInventory.pine -= logsRequired;
        if (plankType == "BirchPlank") woodInventory.birch -= logsRequired;
    }

    bool IsOverlapping(RectTransform a, RectTransform b)
    {
        Rect ra = GetWorldRect(a);
        Rect rb = GetWorldRect(b);
        return ra.Overlaps(rb);
    }

    Rect GetWorldRect(RectTransform rt)
    {
        Vector3[] corners = new Vector3[4];
        rt.GetWorldCorners(corners);
        return new Rect(corners[0], corners[2] - corners[0]);
    }

    float GetLaneX(int lane)
    {
        float width = playAreaRect.rect.width;
        return -width / 2f + (width / (laneCount - 1)) * lane;
    }
}
