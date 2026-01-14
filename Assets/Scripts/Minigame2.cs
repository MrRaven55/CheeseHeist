using UnityEngine;

public class Minigame2 : MonoBehaviour
{
    [Header("Lanes")]
    [SerializeField] private int laneCount = 8;
    [SerializeField] private float laneSpacing = 120f;

    [Header("Player")]
    [SerializeField] private RectTransform gear;
    [SerializeField] private int startLane = 3;

    private int currentLane;

    [Header("Fruit")]
    [SerializeField] private RectTransform fruitPrefab;
    [SerializeField] private RectTransform fruitParent;
    [SerializeField] private float spawnInterval = 1f;

    private float spawnTimer;
    private int lastSpawnLane;

    [Header("Speed")]
    [SerializeField] private float baseFallSpeed = 300f;
    [SerializeField] private float speedMultiplier = 1f;

    [Header("Collection")]
    [SerializeField] private float collectPadding = 10f;

    private RectTransform canvasRect;
    private int score;

    float CurrentFallSpeed => baseFallSpeed * speedMultiplier;

    void OnEnable()
    {
        canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();

        currentLane = startLane;
        lastSpawnLane = startLane;
        spawnTimer = spawnInterval;
        score = 0;
        speedMultiplier = 1f;

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
            spawnTimer = spawnInterval;
        }
    }

    void SpawnFruit()
    {
        int lane = GetNextLane();

        RectTransform fruit = Instantiate(fruitPrefab, fruitParent);
        fruit.gameObject.SetActive(true);

        Vector2 pos = fruit.anchoredPosition;
        pos.x = GetLaneX(lane);
        pos.y = canvasRect.rect.height / 2f + 100f;
        fruit.anchoredPosition = pos;

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

            fruit.anchoredPosition += Vector2.down * CurrentFallSpeed * Time.deltaTime;

            if (IsOverlapping(fruit, gear))
            {
                CollectFruit(fruit);
                continue;
            }

            if (fruit.anchoredPosition.y < gear.anchoredPosition.y - 150f)
            {
                Destroy(fruit.gameObject);
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

    void CollectFruit(RectTransform fruit)
    {
        score++;
        Debug.Log("Collected! Score: " + score);
        Destroy(fruit.gameObject);
    }

    float GetLaneX(int laneIndex)
    {
        float centerOffset = (laneCount - 1) * 0.5f;
        return (laneIndex - centerOffset) * laneSpacing;
    }

    void OnDisable()
    {
        foreach (Transform child in fruitParent)
            Destroy(child.gameObject);
    }
}
