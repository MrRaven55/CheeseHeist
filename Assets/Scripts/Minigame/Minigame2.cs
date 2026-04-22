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
    [SerializeField] private int playerId = -1;

    [Header("Fruit")]
    [SerializeField] private RectTransform fruitPrefab;
    [SerializeField] private RectTransform fruitParent;

    [Header("Inventory")]
    [SerializeField] private WoodInventory woodInventory;

    [Header("Costs")]
    [SerializeField] private int logsRequired = 2;

    [Header("Speed")]
    [SerializeField] private float fallSpeed = 300f;

    [Header("Spawn")]
    [SerializeField] private float spawnInterval = 1f;

    [Header("Win Condition")]
    [SerializeField] private int targetPlanks = 5;
    [SerializeField] private UnityEvent onWin;
    [SerializeField] private UnityEvent onNotEnoughMaterials;

    [Header("Keycodes")]
    [SerializeField] private KeyCode leftKey = KeyCode.A;
    [SerializeField] private KeyCode rightKey = KeyCode.D;


    private RectTransform playAreaRect;
    private int currentLane;
    private int craftedPlanks;
    private float spawnTimer;
    private int lastSpawnLane = 0;

    private void OnEnable()
    {
        SetControlledPlayerMovement(false);

        if (woodInventory == null)
            woodInventory = FindObjectOfType<WoodInventory>();

        playAreaRect = GetComponent<RectTransform>();
        currentLane = Mathf.Clamp(startLane, 0, Mathf.Max(0, laneCount - 1));
        spawnTimer = spawnInterval;
        craftedPlanks = 0;
        UpdateGearPosition();

        if (!HasEnoughRawWoodForPlank())
        {
            EndMinigameNotEnoughMaterials();
        }
    }

    public void ConfigureSession(GameObject player, int controllingPlayerId, WoodInventory inventory)
    {
        if (player != null)
            playerObject = player;

        playerId = controllingPlayerId;

        if (inventory != null)
            woodInventory = inventory;
    }

    private void Update()
    {
        HandleInput();
        HandleSpawning();
        MoveAndCheckFruits();
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(leftKey)) MoveLane(-1);
        if (Input.GetKeyDown(rightKey)) MoveLane(1);
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
            EndMinigameNotEnoughMaterials();
            return;
        }

        // 2 logs of any type -> 1 plank
        if (!woodInventory.TryCraftPlankFromAnyWood(logsRequired))
        {
            EndMinigameNotEnoughMaterials();
            return;
        }

        craftedPlanks++;
        fb.MarkCollected();

        if (craftedPlanks >= targetPlanks)
        {
            SetControlledPlayerMovement(true);
            onWin?.Invoke();
            gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        SetControlledPlayerMovement(true);
    }

    private void SetControlledPlayerMovement(bool enabled)
    {
        if (playerId >= 0)
        {
            PlayerMovement.SetMovementEnabledForPlayer(playerId, enabled);
        }
    }

    private bool HasEnoughRawWoodForPlank()
    {
        if (woodInventory == null) return false;
        int totalRawWood = woodInventory.Oak + woodInventory.Birch + woodInventory.Pine;
        return totalRawWood >= logsRequired;
    }

    private void EndMinigameNotEnoughMaterials()
    {
        SetControlledPlayerMovement(true);
        onNotEnoughMaterials?.Invoke();
        gameObject.SetActive(false);
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
