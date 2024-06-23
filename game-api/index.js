const express = require('express');
const bodyParser = require('body-parser');
const https = require('https');
const fs = require('fs');
const { v4: uuidv4 } = require('uuid');
const jwt = require('jsonwebtoken');

const app = express();
app.use(bodyParser.json());

const secretKey = 'your_secret_key'; // Use a strong secret key in production

let users = [];
let scores = [];
let tokenBlacklist = [];

// Middleware to check if token is blacklisted
const checkTokenBlacklist = (req, res, next) => {
    const token = req.headers['authorization']?.split(' ')[1];
    if (token && tokenBlacklist.includes(token)) {
        return res.status(401).send({ message: 'Token is invalidated' });
    }
    next();
};

// Middleware to authenticate JWT token
const authenticateJWT = (req, res, next) => {
    const token = req.headers['authorization']?.split(' ')[1];
    if (!token) {
        return res.status(401).send({ message: 'Access token is missing or invalid' });
    }

    jwt.verify(token, secretKey, (err, user) => {
        if (err || tokenBlacklist.includes(token)) {
            return res.status(403).send({ message: 'Token is invalid' });
        }
        req.user = user; // Store the user information for later use
        next();
    });
};

// Insert a new user
app.post('/insert-user', (req, res) => {
    const { username, email, hashedPassword } = req.body;

    // Check if the email is already used
    const existingUser = users.find(u => u.email === email);
    if (existingUser) {
        return res.status(400).send({ message: 'Email is already used' });
    }

    // Generate a new userId and set createdAt and lastLogin
    const userId = uuidv4();
    const createdAt = new Date().toISOString();
    const lastLogin = null;

    const newUser = {
        userID: userId,
        username,
        email,
        hashedPassword,
        createdAt,
        lastLogin
    };

    users.push(newUser);
    res.status(201).send(newUser);
});

// Insert or update score
app.post('/insert-score', authenticateJWT, checkTokenBlacklist, (req, res) => {
    const { userID, value } = req.body;

    // Check if the userID exists in the list of registered users
    const userExists = users.find(u => u.userID === userID);
    if (!userExists) {
        return res.status(404).send({ message: 'User not found' });
    }

    // Verify that the userID matches the authenticated user's ID
    if (req.user.userId !== userID) {
        return res.status(403).send({ message: 'User ID does not match the authenticated user' });
    }

    // Current date and time for dateAchieved
    const dateAchieved = new Date().toISOString();

    // Find the score by userID
    const existingScore = scores.find(s => s.userID === userID);
    if (existingScore) {
        // Update the existing score
        existingScore.value = value;
        existingScore.dateAchieved = dateAchieved;
        res.status(200).send(existingScore);
    } else {
        // Insert a new score
        const newScore = {
            id: scores.length + 1,
            userID: userID,
            dateAchieved: dateAchieved,
            value: value
        };
        scores.push(newScore);
        res.status(201).send(newScore);
    }
});

// Get leaderboard (no authorization required)
app.get('/leaderboard', (req, res) => {
    res.send(scores.sort((a, b) => b.value - a.value));
});

// Get user by userId or email (no authorization required)
app.get('/get-user', (req, res) => {
    const { userId, email } = req.query;
    let user;

    switch (true) {
        case !!userId:
            user = users.find(u => u.userID == userId);
            break;
        case !!email:
            user = users.find(u => u.email == email);
            break;
        default:
            return res.status(400).send({ message: 'Please provide either userId or email' });
    }

    if (user) {
        res.send(user);
    } else {
        res.status(404).send({ message: 'User not found' });
    }
});

// Login API
app.post('/login', (req, res) => {
    const { email, password } = req.body;

    const user = users.find(u => u.email === email && u.hashedPassword === password); // Replace with proper password hash check

    if (!user) {
        return res.status(401).send({ message: 'Invalid email or password' });
    }

    // Generate a token
    const token = jwt.sign({ userId: user.userID, email: user.email }, secretKey, { expiresIn: '1h' });

    // Update lastLogin field
    user.lastLogin = new Date().toISOString();

    res.send({ token });
});

// Logout API
app.post('/logout', authenticateJWT, (req, res) => {
    const token = req.headers['authorization']?.split(' ')[1];
    if (token) {
        tokenBlacklist.push(token);
    }
    res.status(200).send({ message: 'Logout successful' });
});

// Read SSL/TLS certificates
const options = {
    key: fs.readFileSync('key.pem'),
    cert: fs.readFileSync('cert.pem')
};

// Create HTTPS server
https.createServer(options, app).listen(3000, () => {
    console.log('HTTPS Server is running on port 3000');
});