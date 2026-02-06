using UnityEngine;

/// <summary>
/// Simple behaviour for falling "fruit" used by Minigame2 (plank crafting).
/// Tracks lane, lifetime and whether it was collected. Does not contain inventory logic.
/// </summary>
public class FruitBehaviour : MonoBehaviour
{
    [HideInInspector] public int laneIndex;
    [HideInInspector] public bool collected;

    private float lifeTime = 10f;
    private float timer;

    /// <summary>
    /// Initialize the fruit after instantiation.
    /// </summary>
    /// <param name="lifeSeconds">Seconds before auto-destroy</param>
    /// <param name="lane">Lane index</param>
    public void Init(float lifeSeconds, int lane)
    {
        lifeTime = lifeSeconds;
        laneIndex = lane;
        timer = 0f;
        collected = false;
    }

    void Update()
    {
        if (collected) return;

        timer += Time.deltaTime;
        if (timer >= lifeTime)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Mark the fruit as collected and remove it.
    /// Minigame2 / inventory should call this when appropriate.
    /// </summary>
    public void MarkCollected()
    {
        if (collected) return;
        collected = true;
        Destroy(gameObject);
    }
}
