using System.Collections;
using UnityEngine;

/// <summary>
/// Simple UI animation that moves a RectTransform from a start point to a target UI corner.
/// Fixed coroutine timing (previously set position immediately and then lerped).
/// </summary>
public class LogMove : MonoBehaviour
{
    [SerializeField] private float moveDuration = 1f;
    [SerializeField] private string uiTagName;                    // optional tag to find target
    [SerializeField] private RectTransform endPointOverride;      // optional explicit target override

    private RectTransform rectTransform;
    private RectTransform endPoint;

    // optional runtime state (keeps track which player triggered the log)
    private int startPosition;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        // prefer explicit override, otherwise try to find by tag
        if (endPointOverride != null)
        {
            endPoint = endPointOverride;
        }
        else if (!string.IsNullOrEmpty(uiTagName))
        {
            GameObject target = GameObject.FindGameObjectWithTag(uiTagName);
            if (target != null)
            {
                endPoint = target.GetComponent<RectTransform>();
            }
            else
            {
                Debug.LogWarning($"LogMove: No object found with tag '{uiTagName}'", this);
            }
        }
    }

    /// <summary>
    /// Starts the UI move from the given anchored start position.
    /// </summary>
    public void StartMove(Vector2 start)
    {
        if (rectTransform == null) rectTransform = GetComponent<RectTransform>();

        rectTransform.anchoredPosition = start;

        if (endPoint == null)
        {
            Debug.LogWarning("LogMove: endPoint is null, cannot move.", this);
            return;
        }

        StartCoroutine(MoveToCorner(endPoint.anchoredPosition));
    }

    /// <summary>
    /// Optional: store which player invoked the movement.
    /// </summary>
    public void Interact(int playerID)
    {
        startPosition = playerID == 1 ? 1 : 2;
    }

    private IEnumerator MoveToCorner(Vector2 target)
    {
        Vector2 start = rectTransform.anchoredPosition;
        float elapsed = 0f;

        // Smoothly interpolate over moveDuration
        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / moveDuration);
            rectTransform.anchoredPosition = Vector2.Lerp(start, target, t);
            yield return null;
        }

        // Ensure exact end position and destroy the UI element
        rectTransform.anchoredPosition = target;
        Destroy(gameObject);
    }
}
