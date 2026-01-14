using UnityEngine;

public class FruitBehaviour : MonoBehaviour
{
    [HideInInspector] public int laneIndex;
    [HideInInspector] public bool collected;

    private float lifeTime = 10f;
    private float timer;

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

    public void MarkCollected()
    {
        if (collected) return;
        collected = true;
        Destroy(gameObject);
    }
}
