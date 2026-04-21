using UnityEngine;

/// <summary>
/// Behaviour for a falling "fruit" used by Minigame2.
/// Tracks lane, lifetime and whether it was collected. Does not contain inventory logic.
/// </summary>
public class FruitBehaviour : MonoBehaviour
{
    // Exposed only in inspector if you want; kept hidden to follow your original layout.
    [HideInInspector] public int laneIndex; // still available in inspector debugging if required

    // Use a property for external checks instead of a public field.
    public bool Collected { get; private set; }

    // Lifetime configuration (private; use Init to set dynamically)
    private float lifeTime = 10f;
    private float timer;

    /// <summary>
    /// Initialize after instantiation. Sets lifetime and lane index.
    /// </summary>
    public void Init(float lifeSeconds, int lane)
    {
        lifeTime = lifeSeconds;
        laneIndex = lane;
        timer = 0f;
        Collected = false;
    }

    private void Update()
    {
        // Do nothing if already collected
        if (Collected) return;

        timer += Time.deltaTime;
        if (timer >= lifeTime)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Mark the fruit as collected and destroy it.
    /// Minigame2 / inventory should call this when appropriate.
    /// </summary>
    public void MarkCollected()
    {
        if (Collected) return;
        Collected = true;
        Destroy(gameObject);
    }
}
