using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Minigame3 : MonoBehaviour
{
    /* ===================== CAMERAS ===================== */
    [Header("Cameras")]
    [SerializeField] private Camera camPlayer1;
    [SerializeField] private Camera camPlayer2;
    [SerializeField] private Camera camMinigame;

    /* ===================== UI / PLANK ===================== */
    [Header("Plank Settings")]
    [Tooltip("Parent RectTransform where planks will spawn")]
    [SerializeField] private RectTransform plankParent;
    [Tooltip("Plank UI prefab (RectTransform). If using child NailPoint1/NailPoint2, place them inside this prefab.")]
    [SerializeField] private RectTransform plankPrefab;
    [SerializeField] private float plankSlideDuration = 0.4f;

    /* ===================== NAIL / HAMMER PREFABS (UI) ===================== */
    [Header("Nail & Hammer Prefabs (UI)")]
    [Tooltip("UI Nail prefab (RectTransform) - e.g. an Image with a RectTransform")]
    [SerializeField] private GameObject nailPrefab;
    [Tooltip("UI Hammer prefab (RectTransform) - e.g. an image of hammer")]
    [SerializeField] private GameObject hammerPrefab;

    /* ===================== ANIMATION SETTINGS ===================== */
    [Header("Animation Timing")]
    [SerializeField] private float nailPlaceDuration = 0.35f;
    [SerializeField] private float hammerSwingDuration = 0.2f;
    [SerializeField] private float hammerReturnDuration = 0.15f;
    [SerializeField] private float hammerHitRotation = -55f;
    [SerializeField] private float nailSinkAmount = 8f; // <--- fixed: define this

    [Header("Spawn Offsets (UI coords)")]
    [SerializeField] private Vector2 nailSpawnOffset = new Vector2(0, 120f);
    [SerializeField] private Vector2 hammerStartOffset = new Vector2(0, 220f);
    [SerializeField] private Vector2 hammerHitOffset = new Vector2(0, -6f);

    /* ===================== NAIL POINTS (INSPECTOR) ===================== */
    [Header("Nail Point Options")]
    [Tooltip("If enabled you can manually set anchored positions for Nail Point 1 & 2 (relative to the plank RectTransform). Otherwise the script will try to find child transforms named NailPoint1 / NailPoint2 inside the plank prefab.")]
    [SerializeField] private bool useCustomNailPoints = false;
    [SerializeField] private Vector2 nailPoint1Position = new Vector2(-40f, 0f);
    [SerializeField] private Vector2 nailPoint2Position = new Vector2(40f, 0f);

    /* ===================== TIMER ===================== */
    [Header("Timer")]
    [SerializeField] private float startTime = 6f;
    [SerializeField] private float timeDecreasePerRound = 0.5f;
    private float currentTime;

    /* ===================== INPUT ===================== */
    [Header("Input")]
    [SerializeField] private string placeKeyP1 = "Fire1";
    [SerializeField] private string hammerKeyP2 = "Fire2";

    /* ===================== STATE ===================== */
    private int nailsPlaced;
    private int nailsHammered;
    private int round;
    private bool minigameActive;
    private bool canPlace;
    private bool canHammer;
    private RectTransform currentPlank;
    private bool plankReady;

    // track spawned nails and whether each is hammered
    private List<RectTransform> spawnedNails = new List<RectTransform>();
    private List<bool> nailIsHammered = new List<bool>();

    /* ===================== UNITY ===================== */

    void Start()
    {
        if (camMinigame) camMinigame.gameObject.SetActive(false);

        if (plankPrefab == null) Debug.LogWarning("Minigame3: plankPrefab is not assigned in inspector.");
        if (plankParent == null) Debug.LogWarning("Minigame3: plankParent is not assigned in inspector.");
        if (nailPrefab == null) Debug.LogWarning("Minigame3: nailPrefab is not assigned in inspector.");
        if (hammerPrefab == null) Debug.LogWarning("Minigame3: hammerPrefab is not assigned in inspector.");
    }

    void Update()
    {
        if (!minigameActive) return;

        HandleTimer();
        HandleInput();
    }

    /* ===================== MINIGAME FLOW ===================== */

    public void StartMinigame()
    {
        minigameActive = true;

        if (camPlayer1) camPlayer1.gameObject.SetActive(false);
        if (camPlayer2) camPlayer2.gameObject.SetActive(false);
        if (camMinigame) camMinigame.gameObject.SetActive(true);

        round = 0;
        StartNextRound();
    }

    private void StartNextRound()
    {
        round++;

        // cleanup any old nails/hammers
        CleanupNails();

        nailsPlaced = 0;
        nailsHammered = 0;
        spawnedNails.Clear();
        nailIsHammered.Clear();

        canPlace = true;
        canHammer = false;

        currentTime = Mathf.Max(1.5f, startTime - (round - 1) * timeDecreasePerRound);

        plankReady = false;
        StartCoroutine(SpawnAndSlidePlank());
    }

    private IEnumerator SpawnAndSlidePlank()
    {
        if (plankPrefab == null || plankParent == null)
        {
            Debug.LogWarning("Minigame3: Missing plankPrefab or plankParent.");
            yield break;
        }

        // instantiate plank UI under plankParent
        RectTransform newPlank = Instantiate(plankPrefab, plankParent);
        // start position: below screen (adjust depending on your layout)
        Vector2 startPos = new Vector2(0, -600f);
        Vector2 endPos = Vector2.zero;
        newPlank.anchoredPosition = startPos;

        float t = 0f;
        while (t < plankSlideDuration)
        {
            t += Time.deltaTime;
            float p = Mathf.SmoothStep(0, 1, t / plankSlideDuration);
            newPlank.anchoredPosition = Vector2.Lerp(startPos, endPos, p);
            yield return null;
        }

        // delete previous plank if exists
        if (currentPlank != null)
            Destroy(currentPlank.gameObject);

        currentPlank = newPlank;
        plankReady = true;

        // ensure lists have capacity for 2 nails
        spawnedNails = new List<RectTransform>(2);
        nailIsHammered = new List<bool>(2);
    }

    /* ===================== INPUT ===================== */

    private void HandleInput()
    {
        // Player 1 places nails
        if (canPlace && Input.GetButtonDown(placeKeyP1))
        {
            PlaceNail();
        }

        // Player 2 hammers nails
        if (Input.GetButtonDown(hammerKeyP2))
        {
            if (!canHammer)
            {
                FailMinigame();
                return;
            }

            // attempt to hammer next not-yet-hammered nail
            HammerNail();
        }
    }

    private void PlaceNail()
    {
        if (!plankReady)
            return;

        if (nailsPlaced >= 2)
            return;

        int indexToPlace = nailsPlaced; // 0 or 1
        nailsPlaced++;

        // reserve a null until coroutine returns
        spawnedNails.Add(null);
        nailIsHammered.Add(false);

        StartCoroutine(PlaceNailCoroutine(indexToPlace));

        if (nailsPlaced >= 2)
        {
            canPlace = false;
            // allow a short delay so P2 reacts visually after second nail finishes placement
            StartCoroutine(EnableHammerAfterDelay(0.05f + nailPlaceDuration));
        }
    }

    private IEnumerator EnableHammerAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        canHammer = true;
    }

    private IEnumerator PlaceNailCoroutine(int nailIndex)
    {
        if (currentPlank == null)
        {
            Debug.LogWarning("PlaceNailCoroutine: currentPlank is null.");
            yield break;
        }

        // get the target anchored position for this nail (relative to the plank RectTransform)
        if (!TryGetNailAnchorPosition(nailIndex + 1, out Vector2 targetAnch))
        {
            Debug.LogWarning("Nail point not found and custom points not set. Nail placement aborted.");
            yield break;
        }

        if (nailPrefab == null)
        {
            Debug.LogWarning("Nail prefab missing.");
            yield break;
        }

        // instantiate nail as UI under the plank (so coordinates match RectTransform)
        GameObject nailGO = Instantiate(nailPrefab, currentPlank);
        RectTransform nailRT = nailGO.GetComponent<RectTransform>();
        if (nailRT == null)
        {
            Debug.LogWarning("Nail prefab has no RectTransform.");
            Destroy(nailGO);
            yield break;
        }

        // start above the target
        Vector2 startAnch = targetAnch + nailSpawnOffset;
        nailRT.anchoredPosition = startAnch;
        nailRT.localRotation = Quaternion.identity;

        float t = 0f;
        while (t < nailPlaceDuration)
        {
            t += Time.deltaTime;
            float p = Mathf.SmoothStep(0, 1, t / nailPlaceDuration);
            nailRT.anchoredPosition = Vector2.Lerp(startAnch, targetAnch, p);
            yield return null;
        }

        // sink it a bit into the plank to look placed
        Vector2 sunkPos = targetAnch + new Vector2(0, -nailSinkAmount);
        float sinkT = 0f;
        float sinkDur = 0.12f;
        Vector2 beforeSink = nailRT.anchoredPosition;
        while (sinkT < sinkDur)
        {
            sinkT += Time.deltaTime;
            float p = Mathf.SmoothStep(0, 1, sinkT / sinkDur);
            nailRT.anchoredPosition = Vector2.Lerp(beforeSink, sunkPos, p);
            yield return null;
        }

        // store reference
        spawnedNails[nailIndex] = nailRT;
        nailIsHammered[nailIndex] = false;
    }

    private void HammerNail()
    {
        // pick first placed nail that is not yet hammered
        int targetIndex = -1;
        for (int i = 0; i < spawnedNails.Count; i++)
        {
            if (spawnedNails[i] != null && !nailIsHammered[i])
            {
                targetIndex = i;
                break;
            }
        }

        if (targetIndex == -1)
        {
            // nothing to hammer (shouldn't happen if canHammer is correctly gated)
            FailMinigame();
            return;
        }

        // mark that hammer action started for this nail to prevent double hammering
        nailIsHammered[targetIndex] = true;

        StartCoroutine(HammerCoroutine(targetIndex));
    }

    private IEnumerator HammerCoroutine(int nailIndex)
    {
        RectTransform targetNail = spawnedNails[nailIndex];
        if (targetNail == null || hammerPrefab == null)
        {
            yield break;
        }

        // instantiate hammer UI under same parent as plank so coordinates align
        GameObject hammerGO = Instantiate(hammerPrefab, currentPlank);
        RectTransform hammerRT = hammerGO.GetComponent<RectTransform>();
        if (hammerRT == null)
        {
            Debug.LogWarning("Hammer prefab has no RectTransform.");
            Destroy(hammerGO);
            yield break;
        }

        // optional: set pivot so rotation looks like a swing (e.g., top-left). You can tweak in prefab.
        hammerRT.localRotation = Quaternion.Euler(0, 0, 0);

        // start position above hammerStartOffset relative to nail
        Vector2 nailAnch = targetNail.anchoredPosition;
        Vector2 startAnch = nailAnch + hammerStartOffset;
        Vector2 hitAnch = nailAnch + hammerHitOffset;

        hammerRT.anchoredPosition = startAnch;
        float t = 0f;

        // swing down (position + rotation)
        while (t < hammerSwingDuration)
        {
            t += Time.deltaTime;
            float p = Mathf.SmoothStep(0, 1, t / hammerSwingDuration);
            hammerRT.anchoredPosition = Vector2.Lerp(startAnch, hitAnch, p);
            float rot = Mathf.Lerp(0f, hammerHitRotation, p);
            hammerRT.localRotation = Quaternion.Euler(0, 0, rot);
            yield return null;
        }

        // small hit feedback on nail: tiny jiggle / deeper sink
        Vector2 originalNailPos = targetNail.anchoredPosition;
        Vector2 deeperPos = originalNailPos + new Vector2(0, -3f);
        float jigT = 0f;
        float jigDur = 0.08f;
        while (jigT < jigDur)
        {
            jigT += Time.deltaTime;
            float p = Mathf.SmoothStep(0, 1, jigT / jigDur);
            targetNail.anchoredPosition = Vector2.Lerp(originalNailPos, deeperPos, p);
            yield return null;
        }
        // return nail a touch
        jigT = 0f;
        while (jigT < jigDur)
        {
            jigT += Time.deltaTime;
            float p = Mathf.SmoothStep(0, 1, jigT / jigDur);
            targetNail.anchoredPosition = Vector2.Lerp(deeperPos, originalNailPos, p);
            yield return null;
        }

        // mark hammered
        nailsHammered++;

        // move hammer up and remove
        t = 0f;
        Vector2 returnPos = startAnch + new Vector2(0, 60f);
        Vector2 beforeReturn = hammerRT.anchoredPosition;
        Quaternion beforeRot = hammerRT.localRotation;
        while (t < hammerReturnDuration)
        {
            t += Time.deltaTime;
            float p = Mathf.SmoothStep(0, 1, t / hammerReturnDuration);
            hammerRT.anchoredPosition = Vector2.Lerp(beforeReturn, returnPos, p);
            hammerRT.localRotation = Quaternion.Slerp(beforeRot, Quaternion.identity, p);
            yield return null;
        }

        Destroy(hammerGO);

        // if round finished (both nails hammered) -> transition to next round
        if (nailsHammered >= 2)
        {
            // small delay so player can see success
            yield return new WaitForSeconds(0.12f);
            StartNextRound();
        }
    }

    /* ===================== TIMER ===================== */

    private void HandleTimer()
    {
        currentTime -= Time.deltaTime;
        if (currentTime <= 0f)
        {
            FailMinigame();
        }
    }

    /* ===================== FAIL / END ===================== */

    private void FailMinigame()
    {
        Debug.Log("Minigame Failed");
        minigameActive = false;
        canPlace = false;
        canHammer = false;

        // stop any running coroutines spawned by this MonoBehaviour
        StopAllCoroutines();

        if (camMinigame) camMinigame.gameObject.SetActive(false);
        if (camPlayer1) camPlayer1.gameObject.SetActive(true);
        if (camPlayer2) camPlayer2.gameObject.SetActive(true);

        CleanupNails();

        if (currentPlank != null)
        {
            Destroy(currentPlank.gameObject);
            currentPlank = null;
        }
    }

    private void CleanupNails()
    {
        foreach (var n in spawnedNails)
        {
            if (n != null)
                Destroy(n.gameObject);
        }
        spawnedNails.Clear();
        nailIsHammered.Clear();
    }

    /* ===================== HELPERS ===================== */

    // Try to get anchored position for nail point (relative to currentPlank).
    // Returns false if neither custom points set nor child transforms found.
    private bool TryGetNailAnchorPosition(int pointNumber, out Vector2 anchoredPos)
    {
        anchoredPos = Vector2.zero;

        // if using custom inspector positions, return them
        if (useCustomNailPoints)
        {
            if (pointNumber == 1) { anchoredPos = nailPoint1Position; return true; }
            if (pointNumber == 2) { anchoredPos = nailPoint2Position; return true; }
            return false;
        }

        // else try to find child transform on the currentPlank
        if (currentPlank == null) return false;

        string nameA = $"NailPoint{pointNumber}";
        Transform child = currentPlank.Find(nameA);
        if (child == null)
        {
            string nameB = $"NailPoint {pointNumber}";
            child = currentPlank.Find(nameB);
        }

        if (child != null)
        {
            RectTransform rt = child as RectTransform;
            if (rt != null)
            {
                anchoredPos = rt.anchoredPosition;
                return true;
            }
        }

        return false;
    }
}
