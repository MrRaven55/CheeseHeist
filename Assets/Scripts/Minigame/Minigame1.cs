using UnityEngine;

public class Minigame1 : MonoBehaviour
{
    [Header("Timer")]
    [SerializeField] private float gameDuration = 30f;
    private float timer;

    [Header("Movement")]
    [SerializeField] private RectTransform movingPiece;
    [SerializeField] private float moveSpeed = 300f; // UI pixels per second
    [SerializeField] private float moveRange = 250f; // pixels up/down

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

    public int score;
    public KeyCode Chop;

    

    private Vector2 startPiecePosition;
    private Vector2 startHitAreaSize;


    void OnEnable()
    {
        Debug.Log("Minigame 1 started");

        timer = gameDuration;

        startPiecePosition = movingPiece.anchoredPosition;
        startHitAreaSize = hitArea.sizeDelta;

        direction = 1;
        score = 0;
    }


    void Update()
    {
        RunTimer();
        MovePiece();
        CheckHit();
    }

    void RunTimer()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            EndMinigame();
        }
    }

    void MovePiece()
    {
        Vector2 pos = movingPiece.anchoredPosition;
        pos.y += direction * moveSpeed * Time.deltaTime;

        if (Mathf.Abs(pos.y - startY) >= moveRange)
        {
            direction *= -1;
        }

        movingPiece.anchoredPosition = pos;
    }

    void CheckHit()
    {
        if (score >= 40)
        {
            EndMinigame();
        }

        if (!Input.GetKeyDown(Chop))
            return;

        if (!IsInsideHitArea())
        {
            Debug.Log("Miss");
            Miss();
            return;
        }

        float currentHitHeight = hitArea.sizeDelta.y;

        if (currentHitHeight <= perfectThresholdHeight)
        {
            score += perfectReward;
            PerfectHit();
            Debug.Log("PERFECT! +" + perfectReward);
        }
        else
        {
            score += normalReward;
            Hit();
            Debug.Log("Good hit +" + normalReward);
        }

        ShrinkHitArea();
    }


    bool IsInsideHitArea()
    {
        float pieceY = movingPiece.anchoredPosition.y;
        float hitY = hitArea.anchoredPosition.y;
        float hitHalfHeight = hitArea.sizeDelta.y / 2f;

        return pieceY >= hitY - hitHalfHeight &&
               pieceY <= hitY + hitHalfHeight;
    }

    void ShrinkHitArea()
    {
        Vector2 size = hitArea.sizeDelta;
        size.y -= hitAreaShrinkAmount;

        if (size.y < minimumHitAreaHeight)
            size.y = minimumHitAreaHeight;

        hitArea.sizeDelta = size;
    }

    void ResetGame()
    {
        timer = gameDuration;
        direction = 1;
        score = 0;

        movingPiece.anchoredPosition = startPiecePosition;
        hitArea.sizeDelta = startHitAreaSize;
    }


    void EndMinigame()
    {
        Debug.Log("Minigame 1 ended");
       
        gameObject.SetActive(false);
    }
    void OnDisable()
    {
        ResetGame();
        Debug.Log("Minigame 1 reset & disabled");
    }

    public void PerfectHit()
    {

    }
    public void Hit()
    {

    }
    public void Miss()
    {

    }

}
