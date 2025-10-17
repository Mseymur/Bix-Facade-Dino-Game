using UnityEngine;
using SocketIOClient;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;  // Add this line

public class SocketIOBridge : MonoBehaviour
{
    public GameManager gameManager;
    public PlayerController playerController;

    private SocketIO socket;

    async void Start()
    {
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

            // Listen for events
            socket.On("startGame", (response) =>
            {
                Debug.Log("Received startGame from server");
                if (gameManager != null)
                {
                    gameManager.StartGame();
                }
            });

            socket.On("jump", (response) =>
            {
                Debug.Log("Received jump from server");
                if (playerController != null)
                {
                    playerController.TryJump();
                }
            });

            await socket.ConnectAsync();
            Debug.Log("Connected to server");
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

    // Call this from GameManager when the game is over
    public async void NotifyGameOver()
    {
        if (socket != null)
        {
            try
            {
                await socket.EmitAsync("gameState", new { gameOver = true });
                Debug.Log("Game over notification sent to server");
            }
            catch (Exception e)
            {
                Debug.LogError($"Error sending game over: {e.Message}");
            }
        }
    }
}