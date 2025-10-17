using UnityEngine;
using UnityEngine.SceneManagement;
using SocketIOClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public enum GameState { Idle, Playing, GameOver }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI Panels")]
    public GameObject startUI;       // Your "Press Space to Start" Text
    public GameObject gameOverUI;    // Your "Game Over – Press Space to Restart" Text
    public GameObject waitingUI;     // New UI for waiting state

    [Header("References")]
    public GameObject player;        // The Dino
    public GameObject spawner;       // Your Spawn point with ObstacleSpawner + GameDifficultyManager

    [Header("Idle Timeout")]
    public float idleReturnTime = 60f;

    GameState currentState = GameState.Idle;
    float gameOverTimer = 0f;
    private SocketIO socket;
    private bool isServerControlled = false;
    private Vector3 initialPlayerPosition;

    void Awake()
    {
        Instance = this;
    }

    async void Start()
    {
        // Store the initial player position
        if (player != null)
        {
            initialPlayerPosition = player.transform.position;
            Debug.Log($"Stored initial player position: {initialPlayerPosition}");
        }
        else
        {
            Debug.LogError("Player reference is missing!");
        }

        try
        {
            // Connect to the server
            var uri = new Uri("http://localhost:3000");
            socket = new SocketIO(uri, new SocketIOOptions
            {
                Query = new Dictionary<string, string>
                {
                    { "type", "unity" }
                }
            });

            // Set up socket event handlers
            socket.On("startGame", (response) => {
                Debug.Log("Received startGame from server");
                // Execute on main thread
                UnityMainThreadDispatcher.Instance().Enqueue(() => StartGame());
            });

            socket.On("jump", (response) => {
                Debug.Log($"Received jump from server. Current state: {currentState}");
                // Execute on main thread
                UnityMainThreadDispatcher.Instance().Enqueue(() => {
                    if (currentState == GameState.Playing)
                    {
                        PlayerController pc = player.GetComponent<PlayerController>();
                        if (pc != null)
                        {
                            Debug.Log("Attempting jump from socket event...");
                            pc.TryJump();
                        }
                        else
                        {
                            Debug.LogError("PlayerController not found on player object!");
                        }
                    }
                    else
                    {
                        Debug.Log($"Jump ignored - not in Playing state. Current state: {currentState}");
                    }
                });
            });

            await socket.ConnectAsync();
            Debug.Log("Connected to server");
            
            // On load, go to Idle
            EnterIdle();
        }
        catch (Exception e)
        {
            Debug.LogError($"Socket connection error: {e.Message}");
        }
    }

    async void OnDestroy()
    {
        if (socket != null)
        {
            await socket.DisconnectAsync();
        }
    }

    void Update()
    {
        // Only handle local input if not server controlled
        if (!isServerControlled)
        {
            // Press SPACE in any state
            if (Input.GetKeyDown(KeyCode.Space))
            {
                switch (currentState)
                {
                    case GameState.Idle:
                        StartGame();             // start the run
                        break;
                    case GameState.Playing:
                        PlayerController pc = player.GetComponent<PlayerController>();
                        if (pc != null) pc.TryJump();  // jump if you can
                        break;
                    case GameState.GameOver:
                        RestartGame();          // reload scene
                        break;
                }
            }
        }

        // If we're dead, count unscaled time to return to Idle
        if (currentState == GameState.GameOver)
        {
            gameOverTimer += Time.unscaledDeltaTime;
            if (gameOverTimer >= idleReturnTime)
                EnterIdle();
        }
    }

    public void GameOver()
    {
        if (currentState != GameState.Playing) return;

        Debug.Log("Game over");
        currentState = GameState.GameOver;

        // Freeze everything
        Time.timeScale = 0f;

        // Show GO UI, hide other UIs
        startUI.SetActive(false);
        gameOverUI.SetActive(true);
        if (waitingUI != null) waitingUI.SetActive(false);

        // Stop spawning new obstacles
        spawner.SetActive(false);

        // Notify server about game over
        if (socket != null)
        {
            Debug.Log("Notifying server of game over state");
            _ = socket.EmitAsync("gameState", new { state = "gameOver", gameOver = true });
        }
    }

    public void StartGame()
    {
        Debug.Log("Starting game...");
        currentState = GameState.Playing;

        // Un-freeze
        Time.timeScale = 1f;

        // Hide all UIs
        startUI.SetActive(false);
        gameOverUI.SetActive(false);
        if (waitingUI != null) waitingUI.SetActive(false);

        // Enable spawning
        spawner.SetActive(true);

        // Notify server that game has started
        if (socket != null)
        {
            Debug.Log("Notifying server of game start");
            _ = socket.EmitAsync("gameState", new { gameStarted = true });
        }

        // Kill the idle-bouncer
        IdleJump idle = player.GetComponent<IdleJump>();
        if (idle != null) idle.StopIdle();

        // Reset player position and velocity
        ResetPlayer();
        
        Debug.Log("Game started successfully");
    }

    void EnterIdle()
    {
        currentState = GameState.Idle;
        Debug.Log("Entering Idle state");

        // 1) Un-freeze
        Time.timeScale = 1f;

        // 2) UI
        startUI.SetActive(true);
        gameOverUI.SetActive(false);
        if (waitingUI != null) waitingUI.SetActive(false);

        // 3) Stop spawning
        spawner.SetActive(false);

        // 4) Reset player
        ResetPlayer();

        // 5) Clear anything left on screen
        ClearObstacles();

        // 6) Kick off idle bouncing
        var idle = player.GetComponent<IdleJump>();
        if (idle != null)
        {
            Debug.Log("Idle start");
            idle.StartIdle();
        }

        // 7) Notify server that game is ready for a new player
        if (socket != null)
        {
            Debug.Log("Notifying server of idle state");
            _ = socket.EmitAsync("gameState", new { state = "idle", gameOver = false });
        }

        // reset the GameOver timer
        gameOverTimer = 0f;
    }

    void ResetPlayer()
    {
        if (player != null)
        {
            // Reset player position to initial position
            Debug.Log($"Resetting player to initial position: {initialPlayerPosition}");
            player.transform.position = initialPlayerPosition;
            
            // Reset player velocity
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = Vector2.zero;
                rb.angularVelocity = 0f;
            }
        }
    }

    void RestartGame()
    {
        // go back to your first scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void ClearObstacles()
    {
        foreach (var o in GameObject.FindGameObjectsWithTag("Obstacle"))
            Destroy(o);
    }

    public bool IsPlaying() => currentState == GameState.Playing;

    // New method to handle server control
    public void SetServerControl(bool serverControlled)
    {
        isServerControlled = serverControlled;
        if (waitingUI != null)
        {
            waitingUI.SetActive(serverControlled);
        }
    }
} 