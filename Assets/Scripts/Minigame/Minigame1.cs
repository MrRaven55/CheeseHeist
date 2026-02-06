using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Minigame that moves a UI piece up/down and the player must "chop" inside a hit area.
/// Scores and rewards wood via WoodInventory. Game resets on disable.
/// </summary>
public class Minigame1 : MonoBehaviour
{
    [Header("Timer")]
    [SerializeField] private float gameDuration = 30f;
    private float timer;

    [Header("Movement")]
    [SerializeField] private RectTransform movingPiece;
    [SerializeField] private float moveSpeed = 300f; // UI px/sec
    [SerializeField] private float moveRange = 250f; // px up/down

    private float startY;
    private int direction = 1;

    [Header("Hit Area")]
    [SerializeField] private RectTransform hitArea;
    [SerializeField] private float hitAreaShrinkAmount = 10f;
    [SerializeField] private float minimumHitAreaHeight = 20f;

    [Header("Rewards")]
    [SerializeField] private float perfectThresholdHeight = 40f; // if hitArea height <= this, it's a perfect hit
    [SerializeField] private int normalReward = 10;
    [SerializeField] private int perfectReward = 20;
    [SerializeField] private int scoreGoal = 40;

    [Header("Inventory / Player (set via script)")]
    [SerializeField] private WoodInventory _woodInventory;
    [SerializeField] private GameObject _playerRef;
    [SerializeField] private string _normalHitWoodType = "Oak";
    [SerializeField] private string _perfectHitWoodType = "OakPlank";

    // expose via properties so PlayerInteraction can set them safely
    public WoodInventory WoodInventory { get => _woodInventory; set => _woodInventory = value; }
    public GameObject PlayerRef { get => _playerRef; set => _playerRef = value; }
    public string NormalHitWoodType { get => _normalHitWoodType; set => _normalHitWoodType = value; }
    public string PerfectHitWoodType { get => _perfectHitWoodType; set => _perfectHitWoodType = value; }

    [Header("Runtime")]
    [SerializeField] private int score;
    [SerializeField] private KeyCode chopKey = KeyCode.Space;

    private Vector2 startPiecePosition;
    private Vector2 startHitAreaSize;

    [Header("Events (Inspector)")]
    [SerializeField] private UnityEvent onPerfect;
    [SerializeField] private UnityEvent onHit;
    [SerializeField] private UnityEvent onMiss;
    [SerializeField] private UnityEvent onEnd;

    private void OnEnable()
    {
        // initialize runtime state
        timer = gameDuration;
        if (movingPiece != null)
        {
            startPiecePosition = movingPiece.anchoredPosition;
            startY = movingPiece.anchoredPosition.y;
        }
        else
        {
            startPiecePosition = Vector2.zero;
            startY = 0f;
        }

        if (hitArea != null) startHitAreaSize = hitArea.sizeDelta;
        direction = 1;
        score = 0;
    }

    private void Update()
    {
        RunTimer();
        MovePiece();
        if (Input.GetKeyDown(chopKey))
            CheckHit();
    }

    private void RunTimer()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
            EndMinigame();
    }

    private void MovePiece()
    {
        if (movingPiece == null) return;

        Vector2 pos = movingPiece.anchoredPosition;
        pos.y += direction * moveSpeed * Time.deltaTime;

        if (Mathf.Abs(pos.y - startY) >= moveRange)
            direction *= -1;

        movingPiece.anchoredPosition = pos;
    }

    /// <summary>
    /// Called when the player attempts a chop. Determines miss/normal/perfect,
    /// adjusts score, shrinks hit area and rewards wood via inventory.
    /// </summary>
    private void CheckHit()
    {
        if (!IsInsideHitArea())
        {
            // Miss
            onMiss?.Invoke();
            Debug.Log("Minigame1: Miss!");
            return;
        }

        // Decide between perfect and normal based on current hitArea size
        if (hitArea != null && hitArea.sizeDelta.y <= perfectThresholdHeight)
        {
            score += perfectReward;
            onPerfect?.Invoke();
            Debug.Log($"Minigame1: PERFECT! +{perfectReward}");

            if (_woodInventory != null && _playerRef != null)
                _woodInventory.AddWood(_perfectHitWoodType, _playerRef);
        }
        else
        {
            score += normalReward;
            onHit?.Invoke();
            Debug.Log($"Minigame1: Good hit +{normalReward}");

            if (_woodInventory != null && _playerRef != null)
                _woodInventory.AddWood(_normalHitWoodType, _playerRef);
        }

        ShrinkHitArea();

        // Cap and end if reached goal
        if (score >= scoreGoal)
        {
            score = scoreGoal;
            EndMinigame();
        }
    }

    private bool IsInsideHitArea()
    {
        if (movingPiece == null || hitArea == null) return false;

        float pieceY = movingPiece.anchoredPosition.y;
        float hitY = hitArea.anchoredPosition.y;
        float hitHalfHeight = hitArea.sizeDelta.y / 2f;
        return pieceY >= (hitY - hitHalfHeight) && pieceY <= (hitY + hitHalfHeight);
    }

    private void ShrinkHitArea()
    {
        if (hitArea == null) return;

        Vector2 size = hitArea.sizeDelta;
        size.y = Mathf.Max(minimumHitAreaHeight, size.y - hitAreaShrinkAmount);
        hitArea.sizeDelta = size;
    }

    private void ResetGame()
    {
        timer = gameDuration;
        direction = 1;
        score = 0;

        if (movingPiece != null) movingPiece.anchoredPosition = startPiecePosition;
        if (hitArea != null) hitArea.sizeDelta = startHitAreaSize;
    }

    private void EndMinigame()
    {
        Debug.Log("Minigame1: ended");
        onEnd?.Invoke();
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        ResetGame();
        Debug.Log("Minigame1: reset & disabled");
    }

    // Inspector-callable callbacks (kept empty so designers can hook events in the inspector)
    public void PerfectHit() { }
    public void Hit() { }
    public void Miss() { }
}
