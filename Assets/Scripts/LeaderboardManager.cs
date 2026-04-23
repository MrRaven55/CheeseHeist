using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using TMPro;

public class LeaderboardManager : MonoBehaviour
{
    [Header("Game Settings")]
    [SerializeField] private string playerName = "Player";
    [SerializeField] private float gameTime = 0f;
    [SerializeField] private bool isCheeseThief = false;
    
    [Header("Leaderboard Settings")]
    [SerializeField] private string leaderboardURL = "https://thegreatcheeseheist.mediacollege.rocks/api.php";
    [SerializeField] private float updateInterval = 5f;
    
    [Header("UI References")]
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private TextMeshProUGUI timerText;
    
    private bool isGameRunning = false;
    private float startTime;

    void Start()
    {
        // Start the timer when game begins
        StartGame();
        
        // Get player name from input field if available
        if (nameInputField != null)
        {
            nameInputField.onEndEdit.AddListener(UpdatePlayerName);
        }
    }

    void Update()
    {
        if (isGameRunning)
        {
            gameTime = Time.time - startTime;
            UpdateTimerDisplay();
        }
        if (Input.GetKey(KeyCode.P))
        {
            StopGame();
        }
    }

    public void StartGame()
    {
        isGameRunning = true;
        startTime = Time.time;
        gameTime = 0f;
    }

    public void StopGame()
    {
        isGameRunning = false;
        SendScoreToLeaderboard();
    }

    public void UpdatePlayerName(string name)
    {
        if (!string.IsNullOrEmpty(name))
        {
            playerName = name;
        }
    }

    private void UpdateTimerDisplay()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(gameTime / 60f);
            int seconds = Mathf.FloorToInt(gameTime % 60f);
            int milliseconds = Mathf.FloorToInt((gameTime * 1000f) % 1000f);
            timerText.text = $"{minutes:00}:{seconds:00}.{milliseconds:000}";
        }
    }

    public void SendScoreToLeaderboard()
    {
        StartCoroutine(PostScore());
    }

    private IEnumerator PostScore()
    {
        // Format time as MM:SS.mmm
        int minutes = Mathf.FloorToInt(gameTime / 60f);
        int seconds = Mathf.FloorToInt(gameTime % 60f);
        int milliseconds = Mathf.FloorToInt((gameTime * 1000f) % 1000f);
        string formattedTime = $"{minutes:00}:{seconds:00}.{milliseconds:000}";

        // Create JSON data
        LeaderboardEntry entry = new LeaderboardEntry
        {
            name = playerName,
            time = formattedTime,
            timeMs = Mathf.FloorToInt(gameTime * 1000f),
            isCheeseThief = isCheeseThief,
            timestamp = System.DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
        };

        string jsonData = JsonUtility.ToJson(entry);
        
        using (UnityWebRequest request = new UnityWebRequest(leaderboardURL, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            
            yield return request.SendWebRequest();
            
            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Score sent to leaderboard successfully!");
                Debug.Log("Response: " + request.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Error sending score to leaderboard: " + request.error);
                Debug.LogError("Response: " + request.downloadHandler.text);
            }
        }
    }

    // Call this method to get current leaderboard data
    public void GetLeaderboardData()
    {
        StartCoroutine(GetLeaderboard());
    }

    private IEnumerator GetLeaderboard()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(leaderboardURL))
        {
            yield return request.SendWebRequest();
            
            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Leaderboard data retrieved: " + request.downloadHandler.text);
                // You can parse and use this data in Unity if needed
            }
            else
            {
                Debug.LogError("Error getting leaderboard data: " + request.error);
            }
        }
    }

    [System.Serializable]
    public class LeaderboardEntry
    {
        public string name;
        public string time;
        public int timeMs;
        public bool isCheeseThief;
        public string timestamp;
    }
}
