using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Two-player nail-and-hammer minigame. Player 1 places nails, Player 2 hammers them.
/// This script spawns UI nails & hammer, handles placement and timing, and advances rounds.
/// </summary>
public class Minigame3 : MonoBehaviour
{
    [Header("Cameras")]
    [SerializeField] private Camera camPlayer1;
    [SerializeField] private Camera camPlayer2;
    [SerializeField] private Camera camMinigame;
    [SerializeField] private CanvasGroup cameraFadeOverlay;
    [SerializeField] private float cameraTransitionDuration = 0.25f;

    [Header("Plank Settings")]
    [Tooltip("Parent RectTransform where planks will spawn")]
    [SerializeField] private RectTransform plankParent;
    [Tooltip("Plank UI prefab (RectTransform). If using child NailPoint1/NailPoint2, place them inside this prefab.")]
    [SerializeField] private RectTransform plankPrefab;
    [SerializeField] private float plankSlideDuration = 0.4f;

    [Header("Nail & Hammer Prefabs (UI)")]
    [SerializeField] private GameObject nailPrefab;
    [SerializeField] private GameObject hammerPrefab;

    [Header("Animation Timing")]
    [SerializeField] private float nailPlaceDuration = 0.35f;
    [SerializeField] private float hammerSwingDuration = 0.2f;
    [SerializeField] private float hammerReturnDuration = 0.15f;
    [SerializeField] private float hammerHitRotation = -55f;
    [SerializeField] private float nailSinkAmount = 8f;

    [Header("Spawn Offsets (UI coords)")]
    [SerializeField] private Vector2 nailSpawnOffset = new Vector2(0, 120f);
    [SerializeField] private Vector2 hammerStartOffset = new Vector2(0, 220f);
    [SerializeField] private Vector2 hammerHitOffset = new Vector2(0, -6f);

    [Header("Nail Point Options")]
    [SerializeField] private bool useCustomNailPoints = false;
    [SerializeField] private Vector2 nailPoint1Position = new Vector2(-40f, 0f);
    [SerializeField] private Vector2 nailPoint2Position = new Vector2(40f, 0f);

    [Header("Timer")]
    [SerializeField] private float startTime = 6f;
    [SerializeField] private float timeDecreasePerRound = 0.5f;

    [Header("Input (strings from Input Manager)")]
    [SerializeField] private string placeKeyP1 = "Fire1";
    [SerializeField] private string hammerKeyP2 = "Fire2";

    [Header("Bridge Progress")]
    [SerializeField] private WoodInventory woodInventory;
    [SerializeField] private int planksPerBridgePiece = 2;
    [SerializeField] private int bridgePiecesToWin = 10;
    [SerializeField] private UnityEvent onBridgePieceBuilt;
    [SerializeField] private UnityEvent onBridgeComplete;

    // runtime state
    private int nailsPlaced;
    private int nailsHammered;
    private int round;
    private bool minigameActive;
    private bool canPlace;
    private bool canHammer;
    private RectTransform currentPlank;
    private bool plankReady;

    public bool IsActive => minigameActive;

    // spawned nails + flags
    private List<RectTransform> spawnedNails = new List<RectTransform>();
    private List<bool> nailIsHammered = new List<bool>();

    private float currentTime;
    private bool startTransitionRunning;

    private void Start()
    {
        if (camMinigame) camMinigame.gameObject.SetActive(false);

        if (cameraFadeOverlay != null)
        {
            cameraFadeOverlay.alpha = 0f;
            cameraFadeOverlay.interactable = false;
            cameraFadeOverlay.blocksRaycasts = false;
        }

        if (woodInventory == null)
            woodInventory = FindObjectOfType<WoodInventory>();

        if (plankPrefab == null) Debug.LogWarning("Minigame3: plankPrefab is not assigned.");
        if (plankParent == null) Debug.LogWarning("Minigame3: plankParent is not assigned.");
        if (nailPrefab == null) Debug.LogWarning("Minigame3: nailPrefab is not assigned.");
        if (hammerPrefab == null) Debug.LogWarning("Minigame3: hammerPrefab is not assigned.");
    }

    private void Update()
    {
        if (!minigameActive) return;

        HandleTimer();
        HandleInput();
    }

    /// <summary>
    /// Caller to start the minigame (activates cameras and initial state).
    /// </summary>
    public void StartMinigame()
    {
        if (startTransitionRunning || minigameActive) return;

        StartCoroutine(StartMinigameRoutine());
    }

    private IEnumerator StartMinigameRoutine()
    {
        startTransitionRunning = true;
        minigameActive = true;
        PlayerMovement.SetAllMovementEnabled(false);

        yield return FadeTo(1f);
        SetCameraMode(true);
        yield return FadeTo(0f);

        round = 0;
        StartNextRound();
        startTransitionRunning = false;
    }

    private void SetCameraMode(bool minigameMode)
    {
        if (camPlayer1) camPlayer1.gameObject.SetActive(!minigameMode);
        if (camPlayer2) camPlayer2.gameObject.SetActive(!minigameMode);
        if (camMinigame) camMinigame.gameObject.SetActive(minigameMode);
    }

    private IEnumerator FadeTo(float targetAlpha)
    {
        if (cameraFadeOverlay == null)
            yield break;

        float duration = Mathf.Max(0.01f, cameraTransitionDuration);
        float startAlpha = cameraFadeOverlay.alpha;
        float t = 0f;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float p = Mathf.Clamp01(t / duration);
            cameraFadeOverlay.alpha = Mathf.Lerp(startAlpha, targetAlpha, p);
            yield return null;
        }

        cameraFadeOverlay.alpha = targetAlpha;
    }

    private void StartNextRound()
    {
        round++;

        // cleanup previous
        CleanupNails();

        nailsPlaced = 0;
        nailsHammered = 0;
        spawnedNails = new List<RectTransform>(2);
        nailIsHammered = new List<bool>(2);

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
            Debug.LogWarning("Minigame3: Missing plank prefab/parent.");
            yield break;
        }

        // instantiate under parent so coordinates are compatible
        RectTransform newPlank = Instantiate(plankPrefab, plankParent);
        // start below the visible area (tweak per UI)
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

        if (currentPlank != null)
            Destroy(currentPlank.gameObject);

        currentPlank = newPlank;
        plankReady = true;
        spawnedNails = new List<RectTransform>(2);
        nailIsHammered = new List<bool>(2);
    }

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

            HammerNail();
        }
    }

    private void PlaceNail()
    {
        if (!plankReady) return;
        if (nailsPlaced >= 2) return;

        int indexToPlace = nailsPlaced;
        nailsPlaced++;

        // reserve slot to be filled by coroutine
        spawnedNails.Add(null);
        nailIsHammered.Add(false);

        StartCoroutine(PlaceNailCoroutine(indexToPlace));

        if (nailsPlaced >= 2)
        {
            canPlace = false;
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

        if (!TryGetNailAnchorPosition(nailIndex + 1, out Vector2 targetAnch))
        {
            Debug.LogWarning("Minigame3: Nail anchor not found.");
            yield break;
        }

        if (nailPrefab == null)
        {
            Debug.LogWarning("Minigame3: nailPrefab missing.");
            yield break;
        }

        GameObject nailGO = Instantiate(nailPrefab, currentPlank);
        RectTransform nailRT = nailGO.GetComponent<RectTransform>();
        if (nailRT == null)
        {
            Debug.LogWarning("Minigame3: nailPrefab has no RectTransform.");
            Destroy(nailGO);
            yield break;
        }

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

        // sink animation
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

        spawnedNails[nailIndex] = nailRT;
        nailIsHammered[nailIndex] = false;
    }

    private void HammerNail()
    {
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
            FailMinigame();
            return;
        }

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

        GameObject hammerGO = Instantiate(hammerPrefab, currentPlank);
        RectTransform hammerRT = hammerGO.GetComponent<RectTransform>();
        if (hammerRT == null)
        {
            Debug.LogWarning("Minigame3: hammerPrefab has no RectTransform.");
            Destroy(hammerGO);
            yield break;
        }

        hammerRT.localRotation = Quaternion.Euler(0, 0, 0);

        Vector2 nailAnch = targetNail.anchoredPosition;
        Vector2 startAnch = nailAnch + hammerStartOffset;
        Vector2 hitAnch = nailAnch + hammerHitOffset;

        hammerRT.anchoredPosition = startAnch;

        float t = 0f;
        while (t < hammerSwingDuration)
        {
            t += Time.deltaTime;
            float p = Mathf.SmoothStep(0, 1, t / hammerSwingDuration);
            hammerRT.anchoredPosition = Vector2.Lerp(startAnch, hitAnch, p);
            float rot = Mathf.Lerp(0f, hammerHitRotation, p);
            hammerRT.localRotation = Quaternion.Euler(0, 0, rot);
            yield return null;
        }

        // nail jiggle / sink effect
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
        jigT = 0f;
        while (jigT < jigDur)
        {
            jigT += Time.deltaTime;
            float p = Mathf.SmoothStep(0, 1, jigT / jigDur);
            targetNail.anchoredPosition = Vector2.Lerp(deeperPos, originalNailPos, p);
            yield return null;
        }

        nailsHammered++;

        // hammer return
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

        // round finished
        if (nailsHammered >= 2)
        {
            if (!BuildBridgePiece())
            {
                FailMinigame();
                yield break;
            }

            if (woodInventory != null && woodInventory.BridgePieces >= bridgePiecesToWin)
            {
                CompleteBridgeMinigame();
                yield break;
            }

            yield return new WaitForSeconds(0.12f);
            StartNextRound();
        }
    }

    private bool BuildBridgePiece()
    {
        if (woodInventory == null)
        {
            Debug.LogWarning("Minigame3: Missing WoodInventory reference.");
            return false;
        }

        bool crafted = woodInventory.TryCraftBridgePiece(planksPerBridgePiece);
        if (crafted)
        {
            onBridgePieceBuilt?.Invoke();
        }
        else
        {
            Debug.Log("Minigame3: Need more planks to build a bridge piece.");
        }

        return crafted;
    }

    private void CompleteBridgeMinigame()
    {
        Debug.Log("Minigame3: Bridge completed. You win!");
        minigameActive = false;
        canPlace = false;
        canHammer = false;
        PlayerMovement.SetAllMovementEnabled(true);

        StopAllCoroutines();
        SetCameraMode(false);

        CleanupNails();

        if (currentPlank != null)
        {
            Destroy(currentPlank.gameObject);
            currentPlank = null;
        }

        onBridgeComplete?.Invoke();
    }

    private void HandleTimer()
    {
        currentTime -= Time.deltaTime;
        if (currentTime <= 0f)
        {
            FailMinigame();
        }
    }

    private void FailMinigame()
    {
        Debug.Log("Minigame3: Failed");
        minigameActive = false;
        canPlace = false;
        canHammer = false;
        PlayerMovement.SetAllMovementEnabled(true);

        StopAllCoroutines();
        SetCameraMode(false);

        CleanupNails();

        if (currentPlank != null)
        {
            Destroy(currentPlank.gameObject);
            currentPlank = null;
        }
    }

    private void OnDisable()
    {
        PlayerMovement.SetAllMovementEnabled(true);
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

    /// <summary>
    /// Try to get anchored position for nail point (1 or 2).
    /// If useCustomNailPoints is true, returns inspector values; otherwise tries to find child transforms named "NailPoint1"/"NailPoint2".
    /// </summary>
    private bool TryGetNailAnchorPosition(int pointNumber, out Vector2 anchoredPos)
    {
        anchoredPos = Vector2.zero;

        if (useCustomNailPoints)
        {
            anchoredPos = pointNumber == 1 ? nailPoint1Position : nailPoint2Position;
            return true;
        }

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
