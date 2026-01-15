using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Minigame2 : MonoBehaviour
{
    [Header("Lanes")]
    [SerializeField] private int laneCount = 8;
    [SerializeField] private RectTransform laneVisualPrefab;      

    [Header("Player")]
    [SerializeField] private RectTransform gear;
    [SerializeField] private int startLane = 3;

    private int currentLane;

    [Header("Fruit")]
    [SerializeField] private RectTransform fruitPrefab;          
    [SerializeField] private RectTransform fruitParent;          

    private int lastSpawnLane;

    [Header("Speed")]
    [SerializeField] private float baseFallSpeed = 300f;
    [SerializeField] private float speedMultiplier = 1f;

    [Header("Spawn Rate")]
    [SerializeField] private float baseSpawnInterval = 1f;
    [SerializeField] private float spawnRateMultiplier = 1f;
    [SerializeField] private float minimumSpawnInterval = 0.25f;

    [Header("Fruit Lifetime")]
    [SerializeField] private float fruitLifetime = 10f; 
    [Header("UI / Collection")]
    [SerializeField] private float collectPadding = 10f;

    [Header("Win Condition")]
    [Tooltip("Score required to end the minigame")]
    [SerializeField] private int targetScore = 30;
    [Tooltip("Optional UnityEvent invoked when targetScore is reached")]
    public UnityEvent onWin;

    private RectTransform canvasRect;
    private RectTransform playAreaRect;
    private float spawnTimer;
    private int score;

    // dynamic spacing
    private float laneSpacingDynamic;
    private Vector2 lastParentSize;
    private RectTransform[] laneVisuals;

    float CurrentFallSpeed => baseFallSpeed * speedMultiplier;
    float CurrentSpawnInterval => Mathf.Max(minimumSpawnInterval, baseSpawnInterval / spawnRateMultiplier);

    void OnEnable()
    {
        canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        playAreaRect = GetComponent<RectTransform>(); // <- self-contained

        currentLane = Mathf.Clamp(startLane, 0, Mathf.Max(0, laneCount - 1));
        lastSpawnLane = currentLane;

        spawnRateMultiplier = Mathf.Max(0.0001f, spawnRateMultiplier);
        speedMultiplier = Mathf.Max(0.0001f, speedMultiplier);
        spawnTimer = CurrentSpawnInterval;

        score = 0;

        ComputeLayout();
        CreateLaneVisuals();
        UpdateGearPosition();
    }

    void Update()
    {
        Vector2 curSize = playAreaRect.rect.size;
        if (curSize != lastParentSize)
            UpdateLayout();

        HandleInput();
        HandleSpawning();
        MoveAndCheckFruits();
    }

    void OnDisable()
    {
        foreach (Transform child in fruitParent)
            Destroy(child.gameObject);

        if (laneVisuals != null)
        {
            foreach (var lv in laneVisuals)
                if (lv != null) Destroy(lv.gameObject);
        }
    }

    void ComputeLayout()
    {
        lastParentSize = playAreaRect.rect.size;
        laneSpacingDynamic = (laneCount > 1) ? playAreaRect.rect.width / (laneCount - 1) : 0f;
    }

    void CreateLaneVisuals()
    {
        if (laneVisuals != null)
        {
            foreach (var l in laneVisuals) if (l != null) Destroy(l.gameObject);
        }

        laneVisuals = new RectTransform[laneCount];

        for (int i = 0; i < laneCount; i++)
        {
            RectTransform lane = Instantiate(laneVisualPrefab, playAreaRect);
            lane.gameObject.SetActive(true);

            lane.anchorMin = lane.anchorMax = new Vector2(0.5f, 0.5f);
            lane.pivot = new Vector2(0.5f, 0.5f);

            Vector2 pos = lane.anchoredPosition;
            pos.x = GetLaneX(i);
            pos.y = 0f;
            lane.anchoredPosition = pos;

            lane.sizeDelta = new Vector2(lane.sizeDelta.x, playAreaRect.rect.height);
            var img = lane.GetComponent<Image>();
            if (img != null) img.color = new Color(1f, 1f, 1f, 0.12f);

            laneVisuals[i] = lane;
        }
    }

    void UpdateLayout()
    {
        ComputeLayout();

        if (laneVisuals != null)
        {
            for (int i = 0; i < laneVisuals.Length; i++)
            {
                if (laneVisuals[i] == null) continue;
                Vector2 pos = laneVisuals[i].anchoredPosition;
                pos.x = GetLaneX(i);
                laneVisuals[i].anchoredPosition = pos;
                laneVisuals[i].sizeDelta = new Vector2(laneVisuals[i].sizeDelta.x, playAreaRect.rect.height);
            }
        }

        UpdateGearPosition();

        for (int i = 0; i < fruitParent.childCount; i++)
        {
            RectTransform child = fruitParent.GetChild(i) as RectTransform;
            if (child == null) continue;
            var fb = child.GetComponent<FruitBehaviour>();
            if (fb != null)
            {
                Vector2 pos = child.anchoredPosition;
                pos.x = GetLaneX(fb.laneIndex);
                child.anchoredPosition = pos;
            }
        }
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            MoveLane(-1);
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            MoveLane(1);
    }

    void MoveLane(int direction)
    {
        currentLane = Mathf.Clamp(currentLane + direction, 0, laneCount - 1);
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
            spawnTimer = CurrentSpawnInterval;
        }
    }

    void SpawnFruit()
    {
        int lane = GetNextLane();

        RectTransform fruit = Instantiate(fruitPrefab, fruitParent);
        fruit.gameObject.SetActive(true);

        Vector2 pos = fruit.anchoredPosition;
        pos.x = GetLaneX(lane);
        pos.y = playAreaRect.rect.height * 0.5f + 100f;
        fruit.anchoredPosition = pos;

        var fb = fruit.GetComponent<FruitBehaviour>();
        if (fb != null)
        {
            fb.Init(fruitLifetime, lane);
        }

        lastSpawnLane = lane;
    }

    int GetNextLane()
    {
        int offset = Random.Range(-1, 2);
        return Mathf.Clamp(lastSpawnLane + offset, 0, laneCount - 1);
    }

    void MoveAndCheckFruits()
    {
        for (int i = fruitParent.childCount - 1; i >= 0; i--)
        {
            RectTransform fruit = fruitParent.GetChild(i) as RectTransform;
            if (fruit == null) continue;

            fruit.anchoredPosition += Vector2.down * CurrentFallSpeed * Time.deltaTime;

            if (IsOverlapping(fruit, gear))
            {
                var fb = fruit.GetComponent<FruitBehaviour>();
                if (fb != null) CollectFruit(fb);
            }
        }
    }

    bool IsOverlapping(RectTransform a, RectTransform b)
    {
        Rect rectA = GetWorldRect(a);
        Rect rectB = GetWorldRect(b);

        rectA.xMin += collectPadding;
        rectA.xMax -= collectPadding;
        rectA.yMin += collectPadding;
        rectA.yMax -= collectPadding;

        return rectA.Overlaps(rectB);
    }

    Rect GetWorldRect(RectTransform rt)
    {
        Vector3[] corners = new Vector3[4];
        rt.GetWorldCorners(corners);
        return new Rect(corners[0], corners[2] - corners[0]);
    }

    void CollectFruit(FruitBehaviour fb)
    {
        if (fb == null || fb.collected) return;
        fb.MarkCollected();
        score++;
        Debug.Log("Collected! Score: " + score);

        if (score >= targetScore)
        {
            EndMinigame();
        }
    }

    float GetLaneX(int laneIndex)
    {
        float width = playAreaRect.rect.width;
        float left = -width * 0.5f;
        return (laneCount > 1) ? left + laneIndex * laneSpacingDynamic : left;
    }

    public void EndMinigame()
    {
        Debug.Log($"Target score {targetScore} reached. Ending minigame.");
        onWin?.Invoke();
        gameObject.SetActive(false);
    }
}
