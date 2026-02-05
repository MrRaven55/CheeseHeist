const express = require('express');
const cors = require('cors');
const path = require('path');

const app = express();
const PORT = 3000;

// In-memory storage for leaderboard data
let leaderboardData = [
    { name: "SpeedRunner", time: "01:23.456", timeMs: 83456, isCheeseThief: true, timestamp: new Date().toISOString() },
    { name: "CheeseLover", time: "01:45.789", timeMs: 105789, isCheeseThief: false, timestamp: new Date().toISOString() },
    { name: "QuickMouse", time: "02:12.345", timeMs: 132345, isCheeseThief: true, timestamp: new Date().toISOString() },
    { name: "CasualPlayer", time: "03:45.678", timeMs: 225678, isCheeseThief: false, timestamp: new Date().toISOString() }
];



// Middleware
app.use(cors());
app.use(express.json());
app.use(express.static(path.join(__dirname, 'test 2-get time from game')));

// API Routes

// Get all leaderboard entries
app.get('/api/leaderboard', (req, res) => {
    // Sort by time (fastest first)
    const sortedData = [...leaderboardData].sort((a, b) => a.timeMs - b.timeMs);
    res.json(sortedData);
});

// Add new leaderboard entry
app.post('/api/leaderboard', (req, res) => {
    const { name, time, timeMs, isCheeseThief, timestamp } = req.body;
    
    // Validate required fields
    if (!name || !time || timeMs === undefined) {
        return res.status(400).json({ error: 'Missing required fields: name, time, timeMs' });
    }
    
    // Create new entry
    const newEntry = {
        name: name.trim(),
        time,
        timeMs,
        isCheeseThief: isCheeseThief || false,
        timestamp: timestamp || new Date().toISOString()
    };
    
    // Add to leaderboard
    leaderboardData.push(newEntry);
    
    // Keep only top 100 entries
    if (leaderboardData.length > 100) {
        leaderboardData.sort((a, b) => a.timeMs - b.timeMs);
        leaderboardData = leaderboardData.slice(0, 100);
    }
    
    console.log('New score added:', newEntry);
    
    res.status(201).json({ 
        message: 'Score added successfully', 
        entry: newEntry,
        rank: leaderboardData.sort((a, b) => a.timeMs - b.timeMs).findIndex(entry => entry === newEntry) + 1
    });
});

// Clear leaderboard (for testing)
app.delete('/api/leaderboard', (req, res) => {
    leaderboardData = [];
    res.json({ message: 'Leaderboard cleared' });
});

// Serve the HTML file
app.get('/', (req, res) => {
    res.sendFile(path.join(__dirname, 'test 2-get time from game', 'game-connection-leaderboard.html'));
});

// Start server
app.listen(PORT, () => {
    console.log(`🧀 Leaderboard server running on http://localhost:${PORT}`);
    console.log(`📊 API endpoints:`);
    console.log(`   GET  /api/leaderboard - Get all scores`);
    console.log(`   POST /api/leaderboard - Add new score`);
    console.log(`   DELETE /api/leaderboard - Clear all scores`);
    console.log(`🌐 Web interface: http://localhost:${PORT}`);
});
