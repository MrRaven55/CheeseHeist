# The Great Cheese Heist - Leaderboard System

A complete leaderboard system for your Unity game that tracks player times and usernames.


### 1. Setup the Server
```bash
# Install dependencies
npm install

# Start the server
npm start
```

The server will run on `http://localhost:3000`

### 2. Unity Setup
1. Copy `LeaderboardManager.cs` to your Unity project
2. Attach the script to a GameObject in your scene
3. Configure the inspector fields:
   - **Player Name**: Link to your input field for player name
   - **Timer Text**: Link to your UI text element for displaying time
   - **Leaderboard URL**: Keep as `http://localhost:3000/api/leaderboard`

### 3. Game Integration
```csharp
// Start timing when game begins
leaderboardManager.StartGame();

// Stop timing and send score when game ends
leaderboardManager.StopGame();

// Update player name from input field
leaderboardManager.UpdatePlayerName("PlayerName");
```

## 📁 Files Overview

- **`LeaderboardManager.cs`** - Unity C# script for game integration
- **`server.js`** - Node.js backend server
- **`game-connection-leaderboard.html`** - Web leaderboard interface
- **`package.json`** - Node.js dependencies

## 🔧 API Endpoints

### GET `/api/leaderboard`
Returns all leaderboard entries sorted by time (fastest first).

**Response:**
```json
[
  {
    "name": "SpeedRunner",
    "time": "01:23.456",
    "timeMs": 83456,
    "isCheeseThief": true,
    "timestamp": "2024-01-01T12:00:00.000Z"
  }
]
```

### POST `/api/leaderboard`
Adds a new score to the leaderboard.

**Request Body:**
```json
{
  "name": "PlayerName",
  "time": "01:23.456",
  "timeMs": 83456,
  "isCheeseThief": false
}
```

## 🎮 Unity Integration Details

### Required Components
- **TextMeshPro** for UI text elements
- **UnityWebRequest** for HTTP communication (included in Unity)

### Key Methods
- `StartGame()` - Begins timing
- `StopGame()` - Ends timing and sends score
- `SendScoreToLeaderboard()` - Manually send current score
- `GetLeaderboardData()` - Fetch current leaderboard

### Time Format
The system uses `MM:SS.mmm` format (minutes:seconds.milliseconds).

## 🌐 Web Interface

Access the leaderboard at `http://localhost:3000` to see:
- Real-time leaderboard updates
- Animated player rankings
- Cheese thief character indicators
- Auto-refresh every 5 seconds

## 🛠️ Development

### Running in Development Mode
```bash
npm run dev
```

### Server Features
- CORS enabled for Unity communication
- In-memory storage (data resets on server restart)
- Automatic sorting by time
- Top 100 scores retention
- Error handling and validation

## 🔍 Troubleshooting

### Unity Can't Connect to Server
1. Ensure the Node.js server is running (`npm start`)
2. Check that port 3000 is not blocked by firewall
3. Verify the URL in Unity inspector matches `http://localhost:3000/api/leaderboard`

### Scores Not Appearing
1. Check Unity Console for error messages
2. Verify server is running and accessible
3. Test the API directly with a tool like Postman

### CORS Errors
The server includes CORS middleware, but if you encounter issues, ensure your Unity build settings allow HTTP requests.

## 📊 Data Structure

Each leaderboard entry contains:
- `name` - Player name (string)
- `time` - Formatted time string (MM:SS.mmm)
- `timeMs` - Time in milliseconds (integer)
- `isCheeseThief` - Boolean flag for character type
- `timestamp` - ISO timestamp of when score was recorded
