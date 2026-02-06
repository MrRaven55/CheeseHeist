using UnityEngine;
using UnityEngine.Events;

public class Minigame1 : MonoBehaviour
{
    [Header("Timer")]
    [SerializeField] private float gameDuration = 30f;
    private float timer;

    [Header("Movement")]
    [SerializeField] private RectTransform movingPiece;
    [SerializeField] private float moveSpeed = 300f; // UI pixels per second
    [SerializeField] private float moveRange = 250f;  // pixels up/down

    private float startY;
    private int direction = 1;

    [Header("Hit Area")]
    [SerializeField] private RectTransform hitArea;
    [SerializeField] private float hitAreaShrinkAmount = 10f;
    [SerializeField] private float minimumHitAreaHeight = 20f;

    [Header("Rewards")]
    [SerializeField] private float perfectThresholdHeight = 40f;
    [SerializeField] private int normalReward = 10;
    [SerializeField] private int perfectReward = 20;
    [SerializeField] private int scoreGoal = 40;

    [Header("Inventory / Player (set via script)")]
    [SerializeField] private WoodInventory _woodInventory;
    [SerializeField] private GameObject _playerRef;
    [SerializeField] private string _normalHitWoodType = "Oak";
    [SerializeField] private string _perfectHitWoodType = "OakPlank";

    // Public getters & setters
    public WoodInventory WoodInventory
    {
        get => _woodInventory;
        set => _woodInventory = value;
    }

    public GameObject PlayerRef
    {
        get => _playerRef;
        set => _playerRef = value;
    }

    public string NormalHitWoodType
    {
        get => _normalHitWoodType;
        set => _normalHitWoodType = value;
    }

    public string PerfectHitWoodType
    {
        get => _perfectHitWoodType;
        set => _perfectHitWoodType = value;
    }

    [Header("Runtime")]
    public int score;
    public KeyCode Chop = KeyCode.Space;

    private Vector2 startPiecePosition;
    private Vector2 startHitAreaSize;

    [Header("Events")]
    public UnityEvent onPerfect;
    public UnityEvent onHit;
    public UnityEvent onMiss;
    public UnityEvent onEnd;

    void OnEnable()
    {
        timer = gameDuration;

        if (movingPiece != null)
            startPiecePosition = movingPiece.anchoredPosition;

        if (hitArea != null)
            startHitAreaSize = hitArea.sizeDelta;

        startY = (movingPiece != null) ? movingPiece.anchoredPosition.y : 0f;
        direction = 1;
        score = 0;
    }

    void Update()
    {
        RunTimer();
        MovePiece();
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

    private void CheckHit()
    {
        if (!Input.GetKeyDown(Chop)) return;

        if (!IsInsideHitArea())
        {
            Miss();
            onMiss?.Invoke();
            Debug.Log("Miss!");
            return;
        }

        // Perfect or normal hit
        if (hitArea != null && hitArea.sizeDelta.y <= perfectThresholdHeight)
        {
            score += perfectReward;
            PerfectHit();
            onPerfect?.Invoke();
            Debug.Log("PERFECT! +" + perfectReward);

            // Reward wood
            if (_woodInventory != null && _playerRef != null)
                _woodInventory.AddWood(_perfectHitWoodType, _playerRef);
        }
        else
        {
            score += normalReward;
            Hit();
            onHit?.Invoke();
            Debug.Log("Good hit +" + normalReward);

            if (_woodInventory != null && _playerRef != null)
                _woodInventory.AddWood(_normalHitWoodType, _playerRef);
        }

        // Shrink hit area
        ShrinkHitArea();

        // Cap score at goal
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

        return pieceY >= hitY - hitHalfHeight && pieceY <= hitY + hitHalfHeight;
    }

    private void ShrinkHitArea()
    {
        if (hitArea == null) return;

        Vector2 size = hitArea.sizeDelta;
        size.y -= hitAreaShrinkAmount;

        if (size.y < minimumHitAreaHeight)
            size.y = minimumHitAreaHeight;

        hitArea.sizeDelta = size;
    }

    private void ResetGame()
    {
        timer = gameDuration;
        direction = 1;
        score = 0;

        if (movingPiece != null)
            movingPiece.anchoredPosition = startPiecePosition;

        if (hitArea != null)
            hitArea.sizeDelta = startHitAreaSize;
    }

    private void EndMinigame()
    {
        Debug.Log("Minigame ended");
        onEnd?.Invoke();
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        ResetGame();
        Debug.Log("Minigame reset & disabled");
    }

    // Callbacks you can subscribe to in the inspector
    public void PerfectHit() { }
    public void Hit() { }
    public void Miss() { }
}
