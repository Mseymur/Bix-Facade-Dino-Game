using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

/// <summary>
/// A helper class to execute actions on the Unity main thread from background threads
/// </summary>
public class UnityMainThreadDispatcher : MonoBehaviour
{
    private static UnityMainThreadDispatcher _instance;
    private readonly Queue<Action> _executionQueue = new Queue<Action>();
    private readonly object _lock = new object();

    public static UnityMainThreadDispatcher Instance()
    {
        if (_instance == null)
        {
            // Create a GameObject with the dispatcher if it doesn't exist
            GameObject go = new GameObject("UnityMainThreadDispatcher");
            _instance = go.AddComponent<UnityMainThreadDispatcher>();
            DontDestroyOnLoad(go);
        }
        return _instance;
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    /// <summary>
    /// Enqueue an action to be executed on the main thread
    /// </summary>
    /// <param name="action">The action to execute</param>
    public void Enqueue(Action action)
    {
        lock (_lock)
        {
            _executionQueue.Enqueue(action);
        }
    }

    /// <summary>
    /// Execute an action on the main thread and wait for it to complete
    /// </summary>
    public async Task EnqueueAsync(Action action)
    {
        TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

        // Wrap the action to set the task as completed when done
        void WrappedAction()
        {
            try
            {
                action();
                tcs.SetResult(true);
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
            }
        }

        Enqueue(WrappedAction);
        await tcs.Task;
    }

    private void Update()
    {
        // Execute all queued actions
        lock (_lock)
        {
            while (_executionQueue.Count > 0)
            {
                Action action = _executionQueue.Dequeue();
                action?.Invoke();
            }
        }
    }
} 